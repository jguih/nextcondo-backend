using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Condominium",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Condominium",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Manager",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified).AddTicks(3269), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified).AddTicks(3269), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Tenant",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified).AddTicks(3265), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified).AddTicks(3266), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Condominium",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Condominium",
                newName: "Created");
        }
    }
}
