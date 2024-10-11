using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCurrentCondo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CurrentCondominium_CondominiumId",
                table: "CurrentCondominium");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentCondominium_CondominiumId",
                table: "CurrentCondominium",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentCondominium_UserId_CondominiumId",
                table: "CurrentCondominium",
                columns: new[] { "UserId", "CondominiumId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CurrentCondominium_CondominiumId",
                table: "CurrentCondominium");

            migrationBuilder.DropIndex(
                name: "IX_CurrentCondominium_UserId_CondominiumId",
                table: "CurrentCondominium");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentCondominium_CondominiumId",
                table: "CurrentCondominium",
                column: "CondominiumId",
                unique: true);
        }
    }
}
