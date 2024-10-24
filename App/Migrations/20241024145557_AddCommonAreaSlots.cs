using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCommonAreaSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SlotId",
                table: "CommonAreaReservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CommonAreaSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name_EN = table.Column<string>(type: "text", nullable: false),
                    Name_PTBR = table.Column<string>(type: "text", nullable: false),
                    CommonAreaId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAreaSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonAreaSlots_CommonAreas_CommonAreaId",
                        column: x => x.CommonAreaId,
                        principalTable: "CommonAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaReservations_SlotId",
                table: "CommonAreaReservations",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaSlots_CommonAreaId",
                table: "CommonAreaSlots",
                column: "CommonAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreaReservations_CommonAreaSlots_SlotId",
                table: "CommonAreaReservations",
                column: "SlotId",
                principalTable: "CommonAreaSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_CommonAreaSlots_SlotId",
                table: "CommonAreaReservations");

            migrationBuilder.DropTable(
                name: "CommonAreaSlots");

            migrationBuilder.DropIndex(
                name: "IX_CommonAreaReservations_SlotId",
                table: "CommonAreaReservations");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "CommonAreaReservations");
        }
    }
}
