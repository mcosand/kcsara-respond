namespace Kcsara.Respond.Model
{
  public class NameIdPair
  {
    public NameIdPair() {}
    public NameIdPair(string id, string name) {
      this.Id = id;
      this.Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
  }
}
