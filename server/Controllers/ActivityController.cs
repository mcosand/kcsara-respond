using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kcsara.Respond.Data;
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
    public async Task<ApiResult<List<ActivityRow>>> List()
    {
      var cutoff = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();

      return new ApiResult<List<ActivityRow>>
      {
        Data = await db.Activities
          .Where(a => a.EndTime == null || a.EndTime > cutoff)
          .OrderByDescending(a => a.StartTime)
          .ToListAsync()
      };
    }
  }
}
