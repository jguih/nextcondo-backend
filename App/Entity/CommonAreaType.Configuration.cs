using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class CommonAreaTypeEntityTypeConfiguration : IEntityTypeConfiguration<CommonAreaType>
{
    public void Configure(EntityTypeBuilder<CommonAreaType> builder)
    {
        builder
            .HasKey(type => type.Id);

        builder
            .Property(type => type.Name_EN)
            .IsRequired();

        builder
            .Property(type => type.Name_PTBR)
            .IsRequired();

        builder
            .HasData(
                new CommonAreaType()
                {
                    Id = 1,
                    Name_EN = "Gym",
                    Name_PTBR = "Academia",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new CommonAreaType()
                {
                    Id = 2,
                    Name_EN = "Swimming Pool",
                    Name_PTBR = "Piscina",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new CommonAreaType()
                {
                    Id = 3,
                    Name_EN = "Laundry Room",
                    Name_PTBR = "Lavanderia",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new CommonAreaType()
                {
                    Id = 4,
                    Name_EN = "Party Hall",
                    Name_PTBR = "Salão de Festas",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new CommonAreaType()
                {
                    Id = 5,
                    Name_EN = "Playground",
                    Name_PTBR = "Parque Infantil",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new CommonAreaType()
                {
                    Id = 6,
                    Name_EN = "Game Room",
                    Name_PTBR = "Sala de Jogos",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new CommonAreaType()
                {
                    Id = 7,
                    Name_EN = "Sports Court",
                    Name_PTBR = "Quadra Esportiva",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                }
            );
    }
}