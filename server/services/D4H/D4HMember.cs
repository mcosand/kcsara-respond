using System.Text.Json.Serialization;

namespace Kcsara.Respond.Services.D4H
{
  public class D4HMember
  {
    public int Id { get; set; }
    public string Ref { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string MobilePhone { get; set; }
    public UrlsType Urls { get; set; }
    public StatusType Status { get; set; }
    [JsonPropertyName("group_ids")]
    public int[] GroupIds { get; set; }

    public class UrlsType
    {
      public string Image { get; set; }
    }

    public class StatusType
    {
      public string Value { get; set; }
    }
  }
}
