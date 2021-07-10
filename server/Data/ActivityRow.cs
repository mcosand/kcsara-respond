using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Kcsara.Respond.Model.GeoJson;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Kcsara.Respond.Data
{
  [Table("Activities")]
  public class ActivityRow
  {
    public Guid Id { get; set; }
    public string Number { get; set; }
    public string Title { get; set; }
    public long StartTime { get; set; }
    public long? EndTime { get; set; }

    public ActivityLocation Location { get; set; }

    public long Created { get; set; }
    public long Updated { get; set; }
    public string Updater { get; set; }

    public ICollection<RespondingUnitRow> Units { get; set; } = new List<RespondingUnitRow>();
  }

  [Owned]
  public class ActivityLocation
  {
    public string Name { get; set; }
    public Geometry Geometry { get; set; }

    public string Wkid { get; set; }

    public string PropertiesJson { get; set; }

    public GeoJsonFeature ToGeoJson() {
      return new GeoJsonFeature
      {
        Id = Wkid,
        Geometry = Geometry.GetViewGeometry(),
        Properties = JsonSerializer.Deserialize<GeoJsonProperties>(PropertiesJson ?? "{}")
      };
    }

    public static ActivityLocation FromGeoJson(GeoJsonFeature feature)
    {
      var location = new ActivityLocation
      {
        Wkid = feature.Id,
        Name = feature.Properties.Title,
        Geometry = feature.Geometry.GetDataGeometry(),
        PropertiesJson = JsonSerializer.Serialize(feature.Properties),
      };

      return location;
    }
  }
}
