using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Features.UsersFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.UsersFeature.Services;

public interface IUsersRepository : IGenericRepository<User>
{
    public Task<User?> GetByEmailAsync(string email);
    public Task<UserDTO?> GetDtoByUserIdAsync(Guid userId);
};

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(
        NextCondoApiDbContext context,
        ILogger<UsersRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var query = from user in entities
                    where user.Email == email
                    select user;
        return await query.FirstOrDefaultAsync();
    }

    public async Task<UserDTO?> GetDtoByUserIdAsync(Guid userId)
    {
        var query = from user in entities
                    where user.Id == userId
                    select new UserDTO()
                    {
                        Id = userId,
                        Email = user.Email,
                        FullName = user.FullName,
                        Phone = user.Phone,
                        Role = new UserDTO.UserRoleDTO()
                        {
                            Name = user.RoleId,
                        },
                    };
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}
