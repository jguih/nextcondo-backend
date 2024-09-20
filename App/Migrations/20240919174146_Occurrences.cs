using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class Occurrences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OccurrenceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_EN = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name_PTBR = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description_EN = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description_PTBR = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OccurrenceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Occurrences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurrenceTypeId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Occurrences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Occurrences_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Occurrences_OccurrenceTypes_OccurrenceTypeId",
                        column: x => x.OccurrenceTypeId,
                        principalTable: "OccurrenceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Occurrences_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "OccurrenceTypes",
                columns: new[] { "Id", "CreatedAt", "Description_EN", "Description_PTBR", "Name_EN", "Name_PTBR", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Issues related to plumbing, electrical, and general repairs.", "Problemas relacionados à encanamento, sistema elétrico e reparos gerais.", "Maintenance", "Manutenção", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Reports of theft, vandalism, or unauthorized access.", "Denúncias de roubo, vandalismo ou acesso não autorizado à propriedade.", "Security Incident", "Incidente de Segurança", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Problems in shared spaces like gyms, pools, or gardens.", "Problemas nas áreas comuns como academias, piscinas ou jardins.", "Common Area", "Área Comum", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Issues related to loud music, parties, or other disturbances.", "Problemas relacionados à som alto, festas ou outros incômodos.", "Noise", "Barulho", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 5, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Fire alarms, medical emergencies, or natural disasters.", "Alarmes de incêndio, emergências médicas ou desastres naturais.", "Emergency", "Emergência", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 6, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Issues with utilities like water, electricity, or internet services.", "Problemas com utilidades como fornecimento de água ou eletricidade ou internet.", "Utility", "Utilidade", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Occurrences_CondominiumId",
                table: "Occurrences",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_Occurrences_CreatorId",
                table: "Occurrences",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Occurrences_OccurrenceTypeId",
                table: "Occurrences",
                column: "OccurrenceTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Occurrences");

            migrationBuilder.DropTable(
                name: "OccurrenceTypes");
        }
    }
}
