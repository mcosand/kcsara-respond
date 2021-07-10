using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kcsara.Respond.Data
{
  [Table("RespondingUnits")]
  public class RespondingUnitRow
  {
    public Guid Id { get; set; }
    public string KnownUnitId { get; set; }
    public string Name { get; set; }
    public long Requested { get; set; }
    public long? Activated { get; set; }

    public Guid ActivityId { get; set; }
    [ForeignKey(name:"ActivityId")]
    public ActivityRow Activity { get; set; }

    public long Updated { get; set; }
    public string Updater { get; set; }
  }
}
