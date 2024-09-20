
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NextCondoApi.Entity;

public class OccurrenceTypeEntityTypeConfiguration : IEntityTypeConfiguration<OccurrenceType>
{
    public void Configure(EntityTypeBuilder<OccurrenceType> builder)
    {
        builder
            .HasKey(t => t.Id);

        builder
            .Property(t => t.Name_EN)
            .HasMaxLength(255);

        builder
            .Property(t => t.Name_PTBR)
            .HasMaxLength(255);

        builder
            .Property(t => t.Description_EN)
            .HasMaxLength(500);

        builder
            .Property(t => t.Description_PTBR)
            .HasMaxLength(500);

        builder
            .HasData(
                new OccurrenceType()
                {
                    Id = 1,
                    Name_EN = "Maintenance",
                    Name_PTBR = "Manutenção",
                    Description_EN = "Issues related to plumbing, electrical, and general repairs.",
                    Description_PTBR = "Problemas relacionados à encanamento, sistema elétrico e reparos gerais.",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new OccurrenceType()
                {
                    Id = 2,
                    Name_EN = "Security Incident",
                    Name_PTBR = "Incidente de Segurança",
                    Description_EN = "Reports of theft, vandalism, or unauthorized access.",
                    Description_PTBR = "Denúncias de roubo, vandalismo ou acesso não autorizado à propriedade.",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new OccurrenceType()
                {
                    Id = 3,
                    Name_EN = "Common Area",
                    Name_PTBR = "Área Comum",
                    Description_EN = "Problems in shared spaces like gyms, pools, or gardens.",
                    Description_PTBR = "Problemas nas áreas comuns como academias, piscinas ou jardins.",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new OccurrenceType()
                {
                    Id = 4,
                    Name_EN = "Noise",
                    Name_PTBR = "Barulho",
                    Description_EN = "Issues related to loud music, parties, or other disturbances.",
                    Description_PTBR = "Problemas relacionados à som alto, festas ou outros incômodos.",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new OccurrenceType()
                {
                    Id = 5,
                    Name_EN = "Emergency",
                    Name_PTBR = "Emergência",
                    Description_EN = "Fire alarms, medical emergencies, or natural disasters.",
                    Description_PTBR = "Alarmes de incêndio, emergências médicas ou desastres naturais.",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                },
                new OccurrenceType()
                {
                    Id = 6,
                    Name_EN = "Utility",
                    Name_PTBR = "Utilidade",
                    Description_EN = "Issues with utilities like water, electricity, or internet services.",
                    Description_PTBR = "Problemas com utilidades como fornecimento de água ou eletricidade ou internet.",
                    CreatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc)),
                    UpdatedAt = new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Utc))
                }
            );
    }
}