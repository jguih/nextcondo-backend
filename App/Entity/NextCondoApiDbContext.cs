
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace NextCondoApi.Entity;

public class NextCondoApiDbContext : DbContext
{
    public NextCondoApiDbContext(
        DbContextOptions<NextCondoApiDbContext> options, 
        IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
    }

    protected IConfiguration Configuration { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
        new RoleEntityTypeConfiguration().Configure(modelBuilder.Entity<Role>());
        new CondominiumEntityTypeConfiguration().Configure(modelBuilder.Entity<Condominium>());
        new CondominiumUserTypeConfiguration().Configure(modelBuilder.Entity<CondominiumUser>());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var ConnectionString = GetConnectionString(Configuration);

        optionsBuilder.UseNpgsql(ConnectionString, (options) =>
        {
            options.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(15),
                errorCodesToAdd: null
            );
        });
        base.OnConfiguring(optionsBuilder);
    }

    public static string GetConnectionString(IConfiguration configuration)
    {
        var HOST = configuration.GetSection("DB_HOST").Get<string>();
        var DATABASE = configuration.GetSection("DB_DATABASE").Get<string>();
        var USER = configuration.GetSection("DB_USER").Get<string>();
        var PASS = configuration.GetSection("DB_PASSWORD").Get<string>();
        var ConnectionStringBuilder = new StringBuilder();

        var ConnectionString = ConnectionStringBuilder
            .Append("Host=").Append(HOST).Append(';')
            .Append("Database=").Append(DATABASE).Append(';')
            .Append("Username=").Append(USER).Append(';')
            .Append("Password=").Append(PASS).Append(';')
            .ToString();

        return ConnectionString;
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        foreach (var entity in entities)
        {
            var now = DateTimeOffset.UtcNow;

            if (entity.State == EntityState.Added)
            {
                ((BaseEntity)entity.Entity).CreatedAt = now;
            }
            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }
    }
}