
using Microsoft.EntityFrameworkCore;

namespace SimplifyCondoApi.Model;

public class SimplifyCondoApiDbContext : DbContext
{
  public SimplifyCondoApiDbContext(DbContextOptions<SimplifyCondoApiDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }
}