using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Kcsara.Respond.Model;
using Kcsara.Respond.Services.D4H;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kcsara.Respond.Services
{
  public class D4HMembersService : ExternalDataService, IMembersService
  {
    List<Group> groups = new List<Group>();
    Dictionary<string, Group> groupsById = new Dictionary<string, Group>();
    Dictionary<string, Group[]> groupsByCategory = new Dictionary<string, Group[]>();
    List<Member> members = new List<Member>();
    Dictionary<string, Member> membersById = new Dictionary<string, Member>();

    static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      WriteIndented = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public List<Group> AllUnits {
      get
      {
        List<Group> groups = new List<Group>();
        lock(this) {
          if (this.groupsByCategory.TryGetValue("Units", out Group[] list)) {
            groups.AddRange(list);
          }
        }
        return groups;
      }
    }

    public D4HMembersService(IConfiguration config, ILogger<D4HMembersService> logger)
    : base("d4hRefreshMinutes", 60, config, logger)
    {
    }

    public Group GetUnit(string id)
    {
      if (this.groupsById.TryGetValue(id, out Group group))
      {
        return group;
      }
      return null;
    }
    
    public Member GetMember(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) return null;
      if (this.membersById.TryGetValue(id, out Member member))
      {
        return member;
      }
      return null;
    }

    protected override async Task RunSync()
    {
      string authToken = config["d4hAuthToken"];
      if (string.IsNullOrWhiteSpace(authToken))
      {
        logger.LogError("d4hAuthToken not specified. Can't load users/groups");
        return;
      }
      HttpClient http = new HttpClient();
      http.BaseAddress = new Uri("https://api.d4h.org/v2/");
      http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
      await DownloadGroups(http);
      await DownloadMembers(http);
      return;
    }

    private async Task DownloadMembers(HttpClient http)
    {
      List<Member> newMembers = new List<Member>();
      Member[] chunk;
      do
      {
        string d = await http.GetStringAsync($"team/members?offset={newMembers.Count}");
        var ds = JsonSerializer.Deserialize<D4HApiResult<D4HMember[]>>(d, jsonOptions);
        chunk = ds.Data.Select(f => new Member
        {
          Id = f.Id.ToString(),
          Name = f.Name,
          Number = f.Ref,
          Email = f.Email,
          Address = f.Address,
          MobilePhone = f.MobilePhone,
          ImageUrl = "https://api.d4h.org" + f.Urls.Image,
          Status = f.Status.Value,
          Groups = f.GroupIds.Select(g => new NameIdPair
          {
            Id = g.ToString(),
            Name = this.groupsById.ContainsKey(g.ToString()) ? this.groupsById[g.ToString()].Name : null
          }).ToList()
        }).ToArray();

        newMembers.AddRange(chunk);
      } while (chunk.Length == 250);

      var lookup = newMembers.ToDictionary(f => f.Id, f => f);

      lock(this)
      {
        this.members = newMembers;
        this.membersById = lookup;
      }
    }

    private async Task DownloadGroups(HttpClient http)
    {
      string[] ignoredGroups = (config["ignoreGroups"] ?? "")
                                      .Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

      List<Group> newGroups = new List<Group>();
      Group[] chunk;
      do
      {
        string d = await http.GetStringAsync($"team/groups?offset={newGroups.Count}");
        var ds = JsonSerializer.Deserialize<D4HApiResult<D4HGroup[]>>(d, jsonOptions);
        chunk = ds.Data.Select(f => new Group
        {
          Id = f.Id.ToString(),
          Category = f.Bundle,
          Name = f.Title
        }).ToArray();

        newGroups.AddRange(chunk);
      } while (chunk.Length == 250);

      newGroups = newGroups.Where(g => ignoredGroups.All(g2 => g.Id != g2)).ToList();
      var lookup = newGroups.ToDictionary(f => f.Id, f => f);
      var categories = newGroups
        .Where(f => !string.IsNullOrWhiteSpace(f.Category))
        .GroupBy(f => f.Category)
        .ToDictionary(f => f.Key, f => f.OrderBy(f => f.Name).ToArray());

      lock(this)
      {
        this.groups = newGroups;
        this.groupsById = lookup;
        this.groupsByCategory = categories;
      }
    }

    public class BackgroundD4HSync : ExternalDataService.BackgroundSyncService<D4HMembersService>
    {
      public BackgroundD4HSync(D4HMembersService service, ILogger<BackgroundD4HSync> logger)
      : base(service, logger)
      {
      }
    }
  }
}