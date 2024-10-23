using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CommonAreaSlotEntityTypeConfiguration : IEntityTypeConfiguration<CommonAreaSlot>
{
    public void Configure(EntityTypeBuilder<CommonAreaSlot> builder)
    {
        builder
            .HasKey(slot => slot.Id);

        builder
            .HasOne(slot => slot.CommonArea)
            .WithMany(commonArea => commonArea.Slots)
            .HasForeignKey(slot => slot.CommonAreaId);
    }
}