using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kcsara.Respond.Model
{
  public class Member : NameIdPair
  {
    public string Number { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string MobilePhone { get; set; }
    [JsonIgnore]
    public string ImageUrl { get; set; }
    public string Status { get; set; }
    public List<NameIdPair> Groups { get; set; }
  }
}
