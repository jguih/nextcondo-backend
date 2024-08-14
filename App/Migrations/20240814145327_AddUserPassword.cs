using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Manager",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Tenant",
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2024, 8, 13, 22, 0, 46, 966, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

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
    }
}
