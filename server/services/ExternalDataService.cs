using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kcsara.Respond.Services
{  
  public abstract class ExternalDataService
  {
    private readonly TimeSpan refreshPeriod;
    protected readonly IConfiguration config;
    protected readonly ILogger<ExternalDataService> logger;
    private ManualResetEvent gate = new ManualResetEvent(true);
    private bool shouldLoop = false;
    private bool isSyncing = false;

    public ExternalDataService(string refreshConfigKey, int defaultRefreshMinutes, IConfiguration config, ILogger<ExternalDataService> logger) {
      this.config = config;
      this.logger = logger;

      if (!int.TryParse(config[refreshConfigKey] ?? defaultRefreshMinutes.ToString(), out int parsedPeriod))
      {
        logger.LogWarning($"Couldn't parse ${refreshConfigKey} value of \"{config[refreshConfigKey]}\". Using default ${defaultRefreshMinutes} minutes.");
        parsedPeriod = defaultRefreshMinutes;
      }
      this.refreshPeriod = TimeSpan.FromMinutes(parsedPeriod);
    }

    protected abstract Task RunSync();
    

    private async Task RunSyncOuter()
    {
      logger.LogInformation("Starting sync for " + this.GetType().Name);
      bool shouldExit;
      lock(this)
      {
        shouldExit = isSyncing;
        isSyncing = true;
      }

      if (shouldExit)
      {
        logger.LogInformation("Was asked to run, but am already running.");
        return;
      }

      try {
        await RunSync();
      } catch (Exception e) {
        logger.LogError(e, "Failure in RunSync");
      }

      lock(this)
      {
        isSyncing = false;
      }
    }

    public void RunNow()
    {
      bool looping;
      lock(this)
      {
        looping = shouldLoop;
        if (looping) gate.Set();
      }

      if (!looping) Task.Run(() => RunSyncOuter());
    }

    private async Task<bool> StartSyncLoop()
    {
      bool goAgain = true;
      lock(this) {
          if (shouldLoop) return false;
          shouldLoop = true;
      }

      while (goAgain) {
        await RunSyncOuter();
        gate.Reset();
        gate.WaitOne(refreshPeriod);
        lock(this)
        {
          goAgain = shouldLoop;
        }
      }
      return true;
    }

    private void StopLoop()
    {
      logger.LogInformation("Stopping loop");
      lock(this)
      {
        shouldLoop = false;
      }
      gate.Set();
    }

    public abstract class BackgroundSyncService<T> : IHostedService where T : ExternalDataService
    {
      private readonly T service;
      private readonly ILogger<BackgroundSyncService<T>> logger;


      public BackgroundSyncService(T service, ILogger<BackgroundSyncService<T>> logger)
      {
        this.service = service;
        this.logger = logger;
      }

      Task IHostedService.StartAsync(CancellationToken cancellationToken)
      {
        Task.Run(async () => {
          bool started = await service.StartSyncLoop();
          if (!started) logger.LogInformation("Loop was already started. No action taken");
        });
        return Task.CompletedTask;
      }

      Task IHostedService.StopAsync(CancellationToken cancellationToken)
      {
        service.StopLoop();
        return Task.CompletedTask;
      }
    }    
  }
}
