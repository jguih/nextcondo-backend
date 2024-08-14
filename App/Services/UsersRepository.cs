using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;

namespace NextCondoApi.Services;

public interface IUsersRepository : IGenericRepository<User>
{
    public Task<User> GetByEmailAsync(string email);
};

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    private readonly IPasswordHasher<User> hasher;
    public UsersRepository(
        NextCondoApiDbContext context, 
        ILogger<UsersRepository> logger,
        IPasswordHasher<User> hasher) 
        : base(context, logger)
    {
        this.hasher = hasher;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var users = await GetAllAsync();
        return users
            .Where((user) => user.Email == email)
            .First();
    }
}
