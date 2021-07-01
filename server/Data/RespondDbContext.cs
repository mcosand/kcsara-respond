using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kcsara.Respond.Data
{
  public abstract class RespondDbContext : DbContext
  {
    protected readonly IConfiguration config;

    public DbSet<ActivityRow> Activities { get; set; }



    public RespondDbContext(IConfiguration config)
    {
      this.config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      SetupPlatform(options);
    }

    protected abstract void SetupPlatform(DbContextOptionsBuilder options);
  }
}