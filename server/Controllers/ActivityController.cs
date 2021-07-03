using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kcsara.Respond.Data;
using Kcsara.Respond.Model.GeoJson;
using Kcsara.Respond.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kcsara.Respond.Controllers
{
  [Authorize]
  [Route("/api/activity")]
  public class ActivityController : Controller
  {
    private readonly RespondDbContext db;

    public ActivityController(RespondDbContext db)
    {
      this.db = db;
    }

    [HttpGet]
    public async Task<ApiResult<List<GetActivityView>>> List()
    {
      var cutoff = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();

      return new ApiResult<List<GetActivityView>>
      {
        Data = (await db.Activities
          .Where(a => a.EndTime == null || a.EndTime > cutoff)
          .OrderByDescending(a => a.StartTime)
          .ToListAsync())
          .Select(RowToView)
          .ToList()
      };
    }

    [HttpPost]
    public async Task<ApiResult<GetActivityView>> Create([FromBody] CreateActivityView data)
    {
      var row = new ActivityRow
      {
        Id = Guid.NewGuid(),
        Title = data.Title,
        Location = ActivityLocation.FromGeoJson(data.Location),
        StartTime = data.StartTime,
        Created = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        Updated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        Updater = User.Identity.Name
      };

      await db.Activities.AddAsync(row);
      await db.SaveChangesAsync();

      return new ApiResult<GetActivityView> { Data = RowToView(row) };
    }

    private GetActivityView RowToView(ActivityRow row)
    {
      var view = new GetActivityView
      {
        Id = row.Id.ToString(),
        Title = row.Title,
        Number = row.Number,
        StartTime = row.StartTime,
        Location = row.Location.ToGeoJson(),
        Updated = row.Updated,
        Updater = row.Updater
      };
      return view;
    }
  }


  public class ActivityViewBase {
    public string Number { get; set; }
    public string Title { get; set; }
    //public ActivityLocation Location { get; set; }
    public List<string> Units { get; set; }
    public long StartTime { get; set; }

    public GeoJsonFeature Location { get; set; }
  }

  public class GetActivityView : ActivityViewBase {
    public string Id { get; set; }
    public string Updater { get; set;}
    public long Updated { get; set; }
  }

  public class CreateActivityView : ActivityViewBase {
    public bool CreateMap { get; set;}
  }
}
