
using Microsoft.EntityFrameworkCore;

namespace server.data.Model
{
  public class DB : DbContext
  {

    public DB(DbContextOptions<DB> options) : base(options) { }

    public DbSet<Forecast> Forecasts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);
      optionsBuilder.UseSqlite("Data Source=./mydb.db;");
    }
  }
}
