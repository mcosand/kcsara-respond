namespace Kcsara.Respond.Model.GeoJson
{
    public class GeoJsonFeature
  {
    public string Id { get; set; }
    public GeoJsonGeometry Geometry { get; set; }
    public GeoJsonProperties Properties { get; set; }
  }
}