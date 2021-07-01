using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kcsara.Respond.Data
{
  [Table("Responders")]
  public class ResponderRow
  {
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Guid UnitId { get; set; }
    [ForeignKey(name:"UnitId")]
    public RespondingUnitRow Unit { get; set; }

    public long Updated { get; set; }
    public string Updater { get; set; }
  }
}
