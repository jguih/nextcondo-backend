
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace NextCondoApi.Entity;

public class NextCondoApiDbContext : DbContext
{
    public NextCondoApiDbContext(DbContextOptions<NextCondoApiDbContext> options, IConfiguration configuration)
        : base(options)
    {
        this.Configuration = configuration;
    }

    private IConfiguration Configuration { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserEntityTypeConfiguration().Configure(modelBuilder.Entity<User>());
        new RoleEntityTypeConfiguration().Configure(modelBuilder.Entity<Role>());
        new CondominiumEntityTypeConfiguration().Configure(modelBuilder.Entity<Condominium>());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var HOST = Configuration.GetSection("DB_HOST").Get<string>();
        var DATABASE = Configuration.GetSection("DB_DATABASE").Get<string>();
        var USER = Configuration.GetSection("DB_USER").Get<string>();
        var PASS = Configuration.GetSection("DB_PASSWORD").Get<string>();
        var ConnectionStringBuilder = new StringBuilder();

        var ConnectionString = ConnectionStringBuilder
            .Append("Host=").Append(HOST).Append(';')
            .Append("Database=").Append(DATABASE).Append(';')
            .Append("Username=").Append(USER).Append(';')
            .Append("Password=").Append(PASS).Append(';')
            .ToString();

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
}