using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kcsara.Respond.Data
{
  public class SqliteRespondDbContext : RespondDbContext
  {
    public static readonly string CONNECTION_CONFIG = "SqliteRespond";

    public SqliteRespondDbContext(IConfiguration config) : base(config)
    {
    }

    protected override void SetupPlatform(DbContextOptionsBuilder options)
    {
      options.UseSqlite(config.GetConnectionString(CONNECTION_CONFIG), x => x.UseNetTopologySuite());
    }
  }
}
