using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kcsara.Respond.Model;
using Kcsara.Respond.Services;
using Kcsara.Respond.ViewModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace Kcsara.Respond.Hubs
{
  public class RespondHub : Hub
  {
    private readonly IMembersService members;
    private readonly IConfiguration config;
    private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public RespondHub(IMembersService members, IConfiguration config)
    {
      this.members = members;
      this.config = config;
    }
    
    public override async Task OnConnectedAsync()
    {
      await base.OnConnectedAsync();

      var user = Context.User;
      var unitBranding = (config["unitDomains"] ?? "")
        .Split(';')
        .Select(unitText => unitText.Split(' '))
        .Where(f => f[0].Equals(Context.GetHttpContext().Request.Host.Value, StringComparison.OrdinalIgnoreCase))
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
          User = user.Identity.IsAuthenticated ? new UserViewModel { Email = user.FindFirst("email").Value, Name =user.Identity.Name } : null,
          Units = members.AllUnits,
          Member = members.GetMember(user.FindFirst("d4hId")?.Value),
          Branding = unitBranding,
          ParentSite = config["parentSite"] ?? "/",
        }
      };

      await Clients.User(Context.UserIdentifier).SendAsync("Initialize", JsonSerializer.Serialize(model, jsonOptions));
    }

    public async Task SendMessage(string user, string message)
    {
      await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
  }
}
