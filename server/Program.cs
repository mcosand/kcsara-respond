using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kcsara.Respond
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureAppConfiguration((hostingContext, config) =>
          {
            config.AddJsonFile("appsettings.local.json", optional: false, reloadOnChange: false);
          })
          .ConfigureLogging(logging =>
          {
            logging.ClearProviders();
            logging.AddConsole();
          })
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseStartup<Startup>();
          });
  }
}
