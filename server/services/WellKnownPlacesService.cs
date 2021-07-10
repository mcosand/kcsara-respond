using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kcsara.Respond.Model.GeoJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kcsara.Respond.Services
{  
  public class WellKnownPlacesService : ExternalDataService
  {
    private readonly IMapService maps;
    private ConcurrentDictionary<string, GeoJsonFeature> knownPlaces = new ConcurrentDictionary<string, GeoJsonFeature>();

    public WellKnownPlacesService(IMapService maps, IConfiguration config, ILogger<WellKnownPlacesService> logger)
    : base("wkpRefreshMinutes", 60, config, logger) 
    {
      this.maps = maps;
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

    protected override async Task RunSync()
    {
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

    public class BackgroundWellKnownPlacesSync : ExternalDataService.BackgroundSyncService<WellKnownPlacesService>
    {
      public BackgroundWellKnownPlacesSync(WellKnownPlacesService service, ILogger<BackgroundWellKnownPlacesSync> logger)
      : base(service, logger)
      {
      }
    }
  }
}
