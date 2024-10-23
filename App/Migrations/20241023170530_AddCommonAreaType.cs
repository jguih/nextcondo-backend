using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCommonAreaType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CommonAreas");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CommonAreas");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "CommonAreas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CommonAreaTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_EN = table.Column<string>(type: "text", nullable: false),
                    Name_PTBR = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAreaTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CommonAreaTypes",
                columns: new[] { "Id", "CreatedAt", "Name_EN", "Name_PTBR", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Gym", "Academia", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Swimming Pool", "Piscina", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Laundry Room", "Lavanderia", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Party Hall", "Salão de Festas", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 5, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Playground", "Parque Infantil", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 6, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Game Room", "Sala de Jogos", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 7, new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Sports Court", "Quadra Esportiva", new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreas_TypeId",
                table: "CommonAreas",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreas_CommonAreaTypes_TypeId",
                table: "CommonAreas",
                column: "TypeId",
                principalTable: "CommonAreaTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreas_CommonAreaTypes_TypeId",
                table: "CommonAreas");

            migrationBuilder.DropTable(
                name: "CommonAreaTypes");

            migrationBuilder.DropIndex(
                name: "IX_CommonAreas_TypeId",
                table: "CommonAreas");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "CommonAreas");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CommonAreas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CommonAreas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
