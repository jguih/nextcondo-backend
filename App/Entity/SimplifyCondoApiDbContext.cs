
using Microsoft.EntityFrameworkCore;

namespace SimplifyCondoApi.Entity;

public class SimplifyCondoApiDbContext : DbContext
{
  public SimplifyCondoApiDbContext(DbContextOptions<SimplifyCondoApiDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users { get; set; }
  public DbSet<Role> Roles { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
    new RoleEntityTypeConfiguration().Configure(modelBuilder.Entity<Role>());
    base.OnModelCreating(modelBuilder);
  }
}