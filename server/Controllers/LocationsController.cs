using System;
using Kcsara.Respond.Data;
using Kcsara.Respond.Services;
using Kcsara.Respond.Model.GeoJson;
using Kcsara.Respond.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kcsara.Respond.Controllers
{
  [Authorize]
  [Route("/api/locations")]
  public class LocationsController : Controller
  {
    private readonly WellKnownPlacesService wkpService;
    private readonly RespondDbContext db;

    public LocationsController(WellKnownPlacesService wkpService, RespondDbContext db)
    {
      this.wkpService = wkpService;
      this.db = db;
    }

    [HttpGet]
    public ApiResult<GeoJsonFeatureCollection> List([FromQuery] string q, [FromQuery] string category) {
      return new ApiResult<GeoJsonFeatureCollection>
      {
        Data = wkpService.Search(q, category)
      };
    }

    [HttpGet]
    [Route("syncnow")]
    public ApiResult<string> SyncNow() {
      wkpService.RunNow();
      return new ApiResult<string> { Data = "OK" };
    }
  }
}
