using System;
using System.Collections.Generic;
using Kcsara.Respond.Model.GeoJson;
using NetTopologySuite.Geometries;

namespace Kcsara.Respond.Data
{
  public static class GeoJsonGeometryDataExtensions
  {
    public static Geometry GetDataGeometry(this GeoJsonGeometry jsonGeometry)
    {
      switch (jsonGeometry.Type)
      {
        case "Point":
          double[] coords = jsonGeometry.GetPointCoordinates();
          return new Point(new Coordinate(coords[0], coords[1]));

        default:
          throw new ApplicationException("Unhandled geometry type " + jsonGeometry.Type);
      }
    }

    public static GeoJsonGeometry GetViewGeometry(this Geometry rowGeometry)
    {
      var json = new GeoJsonGeometry
      {
        Type = rowGeometry.GeometryType
      };

      if (rowGeometry.OgcGeometryType == OgcGeometryType.Point)
      {
        Coordinate c = rowGeometry.Coordinate;
        json.Coordinates = new List<double>(new [] { rowGeometry.Coordinate.X, rowGeometry.Coordinate.Y });
      }
      else
      {
        throw new ApplicationException("Unhandled geometry type in row: " + rowGeometry.GeometryType);
      }
      return json;
    }
  }
}
