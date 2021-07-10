using System.Collections.Generic;
using Kcsara.Respond.Model;

namespace Kcsara.Respond.Services
{  
  public interface IMembersService
  {
    List<Group> AllUnits { get; }
    Group GetUnit(string id);
    Member GetMember(string id);
  }
}
