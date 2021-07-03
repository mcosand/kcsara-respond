using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Kcsara.Respond.Model.GeoJson;
using Kcsara.Respond.Model.SARTopo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kcsara.Respond.Services
{  
  public class SARTopoMapService : IMapService
  {
    private readonly IConfiguration config;
    private readonly ILogger<SARTopoMapService> logger;
    private HttpClient http = new HttpClient();

    public SARTopoMapService(IConfiguration config, ILogger<SARTopoMapService> logger) {
      this.config = config;
      this.logger = logger;
    }

    public async Task<GeoJsonFeatureCollection> GetMapObjects(string mapId)
    {
      var text = await http.GetStringAsync($"https://sartopo.com/api/v1/map/{mapId}/since/0");
      var options = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };
      return JsonSerializer.Deserialize<SARTopoResult<SARTopoMapSince>>(text, options).Result.State;
    }
  }
}
