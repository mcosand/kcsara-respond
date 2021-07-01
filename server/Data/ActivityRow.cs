using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kcsara.Respond.Data
{
  [Table("Activities")]
  public class ActivityRow
  {
    public Guid Id { get; set; }
    public string Number { get; set; }
    public string Title { get; set; }
    public long StartTime { get; set; }
    public long? EndTime { get; set; }

    public long Created { get; set; }
    public long Updated { get; set; }
    public string Updater { get; set; }

    public ICollection<RespondingUnitRow> Units { get; set; }
  }
}
