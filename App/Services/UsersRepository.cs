using NextCondoApi.Entity;

namespace NextCondoApi.Services;

public interface IUsersRepository : IGenericRepository<User>;

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(NextCondoApiDbContext context, ILogger<UsersRepository> logger) : base(context, logger)
    {
    }
}
