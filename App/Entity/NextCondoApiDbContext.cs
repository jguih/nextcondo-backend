
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextCondoApi.Features.Configuration;
using System.Text;

namespace NextCondoApi.Entity;

public class NextCondoApiDbContext : DbContext
{
    public NextCondoApiDbContext(
        DbContextOptions<NextCondoApiDbContext> options, 
        IOptions<DbOptions> dbConfig)
        : base(options)
    {
        _dbConfig = dbConfig;
    }

    private readonly IOptions<DbOptions> _dbConfig;
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Condominium> Condominiums { get; set; }
    public DbSet<CondominiumUser> CondominiumUsers { get; set; }
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
        new RoleEntityTypeConfiguration().Configure(modelBuilder.Entity<Role>());
        new CondominiumEntityTypeConfiguration().Configure(modelBuilder.Entity<Condominium>());
        new CondominiumUserEntityTypeConfiguration().Configure(modelBuilder.Entity<CondominiumUser>());
        new EmailVerificationCodeEntityTypeConfiguration().Configure(modelBuilder.Entity<EmailVerificationCode>());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var ConnectionString = GetConnectionString(_dbConfig);

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

    public static string GetConnectionString(IOptions<DbOptions> dbConfig)
    {
        var HOST = dbConfig.Value.HOST;
        var DATABASE = dbConfig.Value.DATABASE;
        var USER = dbConfig.Value.USER;
        var PASS = dbConfig.Value.PASSWORD;
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