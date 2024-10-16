using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CommonAreaReservationEntityTypeConfiguration : IEntityTypeConfiguration<CommonAreaReservation>
{
    public void Configure(EntityTypeBuilder<CommonAreaReservation> builder)
    {
        builder
            .HasKey(reservation => reservation.Id);

        builder
            .Property(reservation => reservation.Date)
            .IsRequired();

        builder
            .Property(reservation => reservation.StartAt)
            .IsRequired();

        builder
            .HasOne(reservation => reservation.CommonArea)
            .WithMany()
            .HasForeignKey(reservation => reservation.CommonAreaId);

        builder
            .HasOne(reservation => reservation.User)
            .WithMany()
            .HasForeignKey(reservation => reservation.UserId);
    }
}