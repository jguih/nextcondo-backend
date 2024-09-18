using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;

namespace NextCondoApi.Services;

public interface IEmailVerificationCodeRepository : IGenericRepository<EmailVerificationCode>
{
    public Task<int> DeleteEmailCodesForUserAsync(Guid userId);
    public Task<EmailVerificationCode?> GetEmailCodeForUser(Guid userId);
}

public class EmailVerificationCodeRepository : GenericRepository<EmailVerificationCode>, IEmailVerificationCodeRepository
{
    public EmailVerificationCodeRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<EmailVerificationCode>> logger)
        : base(context, logger)
    {
    }

    public async Task<int> DeleteEmailCodesForUserAsync(Guid userId)
    {
        return await (from emailCode in entities
                      where emailCode.UserId == userId
                      select emailCode)
                      .ExecuteDeleteAsync();
    }

    public async Task<EmailVerificationCode?> GetEmailCodeForUser(Guid userId)
    {
        return await (from emailCode in entities
                      where emailCode.UserId == userId
                      select emailCode)
                      .AsNoTracking()
                      .FirstOrDefaultAsync();
    }
}
