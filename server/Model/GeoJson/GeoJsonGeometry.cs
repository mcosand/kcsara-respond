using System;
using System.Linq;
using System.Text.Json;

namespace Kcsara.Respond.Model.GeoJson
{
  public class GeoJsonGeometry
  {
    public string Type { get; set; }
    public object Coordinates { get; set; }

    public double[] GetPointCoordinates()
    {
      if (Type != "Point") throw new ApplicationException("Geometry is not a point");
      if (Coordinates is JsonElement) {
        return ((JsonElement)Coordinates).EnumerateArray().Select(f => f.GetDouble()).ToArray();
      }
      throw new ApplicationException("Unknown storage type " + Coordinates.GetType().Name);
    }
  }
}