using System.Linq;
using Kcsara.Respond.Model;
using Kcsara.Respond.Services;
using Kcsara.Respond.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Kcsara.Respond.Controllers
{
  public class HomeController : Controller
  {
    [Route("/api/startup")]
    public ApiResultViewModel<StartupViewModel> StartupData([FromServices] IMembersService members, [FromServices] IConfiguration config)
    {
      var unitBranding = (config["unitDomains"] ?? "")
        .Split(';')
        .Select(unitText => unitText.Split(' '))
        .Where(f => f[0].Equals(Request.Host.Value, System.StringComparison.OrdinalIgnoreCase))
        .Select(parts => new UnitBranding
          {
            Id = parts[1],
            Color = parts[2],
            Name = members.GetUnit(parts[1])?.Name
          })
          .FirstOrDefault();

      var model = new ApiResultViewModel<StartupViewModel>
      {
        Data = new StartupViewModel
        {
          User = User.Identity.IsAuthenticated ? new UserViewModel { Email = User.FindFirst("email").Value, Name = User.Identity.Name } : null,
          Units = members.AllUnits,
          Member = members.GetMember(User.FindFirst("d4hId")?.Value),
          Branding = unitBranding
        }
      };

      return model;
    }
  }
}
