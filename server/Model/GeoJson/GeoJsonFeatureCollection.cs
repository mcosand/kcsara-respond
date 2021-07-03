using System.Collections.Generic;

namespace Kcsara.Respond.Model.GeoJson
{
  public class GeoJsonFeatureCollection
  {
    public string Type = "FeatureCollection";
    public List<GeoJsonFeature> Features { get; set; }

    public GeoJsonFeatureCollection() : this(new List<GeoJsonFeature>())
    {
    }

    public GeoJsonFeatureCollection(List<GeoJsonFeature> features)
    {
      this.Features = features;
    }
  }
}