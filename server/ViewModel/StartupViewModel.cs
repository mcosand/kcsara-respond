using System;
using System.Collections.Generic;
using Kcsara.Respond.Model;

namespace Kcsara.Respond.ViewModel
{
  public class StartupViewModel
  {
    public UserViewModel User { get; set; }
    public List<Group> Units { get; set; }
    public Member Member { get; set; }
    public UnitBranding Branding { get; set; }
    public string ParentSite { get; set; }
  }
}
