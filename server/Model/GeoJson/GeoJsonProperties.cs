using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kcsara.Respond.Model.GeoJson
{
  public class GeoJsonProperties
  {
    public string Title { get; set; }
    public string Description { get; set; }
    [JsonExtensionData]
    public Dictionary<string, object> More { get; set;}
  }
}