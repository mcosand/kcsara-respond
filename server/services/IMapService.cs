using System.Threading.Tasks;
using Kcsara.Respond.Model.GeoJson;

namespace Kcsara.Respond.Services
{  
  public interface IMapService
  {
    Task<GeoJsonFeatureCollection> GetMapObjects(string mapId);  
  }
}
