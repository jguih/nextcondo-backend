using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCondominiumOwnerFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Condominium_OwnerId",
                table: "Condominium");

            migrationBuilder.CreateIndex(
                name: "IX_Condominium_OwnerId",
                table: "Condominium",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Condominium_OwnerId",
                table: "Condominium");

            migrationBuilder.CreateIndex(
                name: "IX_Condominium_OwnerId",
                table: "Condominium",
                column: "OwnerId",
                unique: true);
        }
    }
}
