using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class EmailVerificationCodeEntityTypeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
    {
        builder
            .HasKey(code => code.Id);

        builder
            .Property(code => code.Code)
            .IsRequired();

        builder
            .Property(code => code.Email)
            .IsRequired();

        builder
            .Property(code => code.ExpirestAt)
            .IsRequired();
        

        builder
            .HasOne(code => code.User)
            .WithOne()
            .HasForeignKey<EmailVerificationCode>(code => code.UserId);
    }
}
