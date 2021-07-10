using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kcsara.Respond.Data;
using Kcsara.Respond.Model;
using Kcsara.Respond.Model.GeoJson;
using Kcsara.Respond.Services;
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
    public async Task<ApiResultViewModel<List<GetActivityView>>> List()
    {
      var cutoff = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();

      return new ApiResultViewModel<List<GetActivityView>>
      {
        Data = (await db.Activities
          .Include(f => f.Units)
          .Where(a => a.EndTime == null || a.EndTime > cutoff)
          .OrderByDescending(a => a.StartTime)
          .ToListAsync())
          .Select(RowToView)
          .ToList()
      };
    }

    [HttpPost]
    public async Task<ApiResultViewModel<GetActivityView>> Create([FromBody] CreateActivityView data, [FromServices] IMembersService members)
    {
      var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      var row = new ActivityRow
      {
        Id = Guid.NewGuid(),
        Number = data.Number,
        Title = data.Title,
        Location = ActivityLocation.FromGeoJson(data.Location),
        StartTime = data.StartTime,
        Created = now,
        Updated = now,
        Updater = User.Identity.Name
      };

      foreach (var unitId in data.Units)
      {
        var unit = members.GetUnit(unitId);
        if (unit == null) continue;

        var unitRow = new RespondingUnitRow
        {
          Id = Guid.NewGuid(),
          KnownUnitId = unit.Id,
          Name = unit.Name,
          Activity = row,
          Updated = now,
          Updater = User.Identity.Name
        };
        row.Units.Add(unitRow);
      }

      await db.Activities.AddAsync(row);
      await db.SaveChangesAsync();

      return new ApiResultViewModel<GetActivityView> { Data = RowToView(row) };
    }

    private GetActivityView RowToView(ActivityRow row)
    {
      var oldest = Math.Min(row.Units.Select(f => f.Requested).DefaultIfEmpty().Min(), row.StartTime);
      var view = new GetActivityView
      {
        Id = row.Id.ToString(),
        Title = row.Title,
        Number = row.Number,
        StartTime = row.StartTime,
        EndTime = row.EndTime,
        Location = row.Location.ToGeoJson(),
        Units = row.Units
          .OrderBy(f => f.Activated ?? oldest)
          .ThenBy(f => f.Requested)
          .ThenBy(f => f.Name)
          .Select(u => new RespondingUnitRow
          {
            Id = u.Id,
            KnownUnitId = u.KnownUnitId,
            Name = u.Name,
          }).ToList(),
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
    public long StartTime { get; set; }
    public long? EndTime { get; internal set; }
    public GeoJsonFeature Location { get; set; }
  }

  public class GetActivityView : ActivityViewBase {
    public string Id { get; set; }
    public List<RespondingUnitRow> Units { get; set; }
    public string Updater { get; set;}
    public long Updated { get; set; }
  }

  public class CreateActivityView : ActivityViewBase {
    public bool CreateMap { get; set;}
    public List<string> Units { get; set; }
  }
}
