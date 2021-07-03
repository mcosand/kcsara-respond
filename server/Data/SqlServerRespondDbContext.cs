using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kcsara.Respond.Data
{  public class SqlServerRespondDbContext : RespondDbContext
  {
    public static readonly string CONNECTION_CONFIG = "SqlServerRespond";

    public SqlServerRespondDbContext(IConfiguration config) : base(config)
    {
    }

    protected override void SetupPlatform(DbContextOptionsBuilder options)
    {
      options.UseSqlServer(config.GetConnectionString(CONNECTION_CONFIG), x => x.UseNetTopologySuite());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasDefaultSchema("respond");
    }
  }
}
