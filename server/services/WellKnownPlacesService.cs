using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kcsara.Respond.Model.GeoJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kcsara.Respond.Services
{  
  public class WellKnownPlacesService
  {
    private static readonly TimeSpan PERIOD = TimeSpan.FromMinutes(10);
    private readonly IMapService maps;
    private readonly IConfiguration config;
    private readonly ILogger<WellKnownPlacesService> logger;
    private ConcurrentDictionary<string, GeoJsonFeature> knownPlaces = new ConcurrentDictionary<string, GeoJsonFeature>();
    private ManualResetEvent gate = new ManualResetEvent(true);
    private bool shouldLoop = false;
    private bool isSyncing = false;

    public WellKnownPlacesService(IMapService maps, IConfiguration config, ILogger<WellKnownPlacesService> logger) {
      this.maps = maps;
      this.config = config;
      this.logger = logger;
    }

    public GeoJsonFeatureCollection Search(string query, string category)
    {
      return new GeoJsonFeatureCollection(knownPlaces.Values
        .Where(f => {
          if (string.IsNullOrWhiteSpace(category)) return true;
          var categories = new List<string>();
          if (f.Properties.More.ContainsKey("categories")) categories.AddRange((IEnumerable<string>)f.Properties.More["categories"]);
          for (var i=0; i<categories.Count; i++)
          {
            if (category.Equals(categories[i], StringComparison.OrdinalIgnoreCase))
            {
              return true;
            }
          }
          return false;
        })
        .Where(f => string.IsNullOrWhiteSpace(query)
                    || f.Properties.Title.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                    || (f.Properties.Description?.Contains(query, StringComparison.InvariantCultureIgnoreCase) ?? false))
        .ToList()
      );
    }

    private async Task SyncMapsOnce()
    {
      logger.LogInformation("Starting SyncMapsOnce");
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

      logger.LogInformation("Syncing well-known places map(s) ...");
      string mapId = null;
      try {
        var mapIds = (config["wkpMapIds"] ?? "").Split(",").Select(f => f.ToUpperInvariant().Trim());
        
        var oldFeatures = new List<string>(knownPlaces.Keys);
        foreach (var innerMapId in mapIds)
        {
          mapId = innerMapId;
          logger.LogDebug($"Downloading map {mapId} ...");
          var features = (await maps.GetMapObjects(mapId)).Features;

          foreach (var feature in features.Where(f => f.Geometry != null))
          {
            // If there's a line in the description with the string "Ignore", skip the object.
            if ((feature.Properties.Description
                  ?.Split("\n", StringSplitOptions.TrimEntries) ?? new string[0])
                  .Any(f => f.Equals("Ignore", StringComparison.InvariantCultureIgnoreCase)))
            {
              continue;
            }
            feature.Properties.More.Add("categories", GetCategories(feature));
            knownPlaces[feature.Id] = feature;
            oldFeatures.Remove(feature.Id);
          }
        }

        foreach (var oldId in oldFeatures)
        {
          knownPlaces.Remove(oldId, out GeoJsonFeature _);
        }

        logger.LogInformation($"Sync finished with {knownPlaces.Count} total places.");
      } catch (Exception e) {
        logger.LogError(e, $"While syncing map {mapId}");
      }
      lock(this)
      {
        isSyncing = false;
      }
    }

    private List<string> GetCategories(GeoJsonFeature feature)
    {
      var comments = feature.Properties.Description?.ToString() ?? string.Empty;
      var categoryLine = comments.Split("\n").FirstOrDefault(f => f.ToUpperInvariant().StartsWith("CATEGORY:")) ?? "Category: Other";
      return categoryLine
        .Split(new char[] { ' ', ',', ':' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Skip(1) // Get rid of category header
        .ToList();
    }

    public void RunNow()
    {
      bool looping;
      lock(this)
      {
        looping = shouldLoop;
        if (looping) gate.Set();
      }

      if (!looping) Task.Run(() => SyncMapsOnce());
    }

    private async Task<bool> StartSyncLoop()
    {
      bool goAgain = true;
      lock(this) {
          if (shouldLoop) return false;
          shouldLoop = true;
      }

      while (goAgain) {
        await SyncMapsOnce();
        gate.Reset();
        gate.WaitOne(PERIOD);
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

    public class BackgroundWellKnownPlacesSync : IHostedService
    {
      private readonly WellKnownPlacesService service;
      private readonly ILogger<BackgroundWellKnownPlacesSync> logger;


      public BackgroundWellKnownPlacesSync(WellKnownPlacesService service, ILogger<BackgroundWellKnownPlacesSync> logger)
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
