using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextCondoApi.Migrations
{
    /// <inheritdoc />
    public partial class FixTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Condominium_Users_OwnerId",
                table: "Condominium");

            migrationBuilder.DropForeignKey(
                name: "FK_CondominiumUser_Condominium_CondominiumId",
                table: "CondominiumUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CondominiumUser_Users_UserId",
                table: "CondominiumUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationCode_Users_UserId",
                table: "EmailVerificationCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailVerificationCode",
                table: "EmailVerificationCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CondominiumUser",
                table: "CondominiumUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Condominium",
                table: "Condominium");

            migrationBuilder.RenameTable(
                name: "EmailVerificationCode",
                newName: "EmailVerificationCodes");

            migrationBuilder.RenameTable(
                name: "CondominiumUser",
                newName: "CondominiumUsers");

            migrationBuilder.RenameTable(
                name: "Condominium",
                newName: "Condominiums");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerificationCode_UserId",
                table: "EmailVerificationCodes",
                newName: "IX_EmailVerificationCodes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CondominiumUser_CondominiumId",
                table: "CondominiumUsers",
                newName: "IX_CondominiumUsers_CondominiumId");

            migrationBuilder.RenameIndex(
                name: "IX_Condominium_OwnerId",
                table: "Condominiums",
                newName: "IX_Condominiums_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailVerificationCodes",
                table: "EmailVerificationCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CondominiumUsers",
                table: "CondominiumUsers",
                columns: new[] { "UserId", "RelationshipType", "CondominiumId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Condominiums",
                table: "Condominiums",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Condominiums_Users_OwnerId",
                table: "Condominiums",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CondominiumUsers_Condominiums_CondominiumId",
                table: "CondominiumUsers",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CondominiumUsers_Users_UserId",
                table: "CondominiumUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationCodes_Users_UserId",
                table: "EmailVerificationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Condominiums_Users_OwnerId",
                table: "Condominiums");

            migrationBuilder.DropForeignKey(
                name: "FK_CondominiumUsers_Condominiums_CondominiumId",
                table: "CondominiumUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CondominiumUsers_Users_UserId",
                table: "CondominiumUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationCodes_Users_UserId",
                table: "EmailVerificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailVerificationCodes",
                table: "EmailVerificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CondominiumUsers",
                table: "CondominiumUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Condominiums",
                table: "Condominiums");

            migrationBuilder.RenameTable(
                name: "EmailVerificationCodes",
                newName: "EmailVerificationCode");

            migrationBuilder.RenameTable(
                name: "CondominiumUsers",
                newName: "CondominiumUser");

            migrationBuilder.RenameTable(
                name: "Condominiums",
                newName: "Condominium");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerificationCodes_UserId",
                table: "EmailVerificationCode",
                newName: "IX_EmailVerificationCode_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CondominiumUsers_CondominiumId",
                table: "CondominiumUser",
                newName: "IX_CondominiumUser_CondominiumId");

            migrationBuilder.RenameIndex(
                name: "IX_Condominiums_OwnerId",
                table: "Condominium",
                newName: "IX_Condominium_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailVerificationCode",
                table: "EmailVerificationCode",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CondominiumUser",
                table: "CondominiumUser",
                columns: new[] { "UserId", "RelationshipType", "CondominiumId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Condominium",
                table: "Condominium",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Condominium_Users_OwnerId",
                table: "Condominium",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CondominiumUser_Condominium_CondominiumId",
                table: "CondominiumUser",
                column: "CondominiumId",
                principalTable: "Condominium",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CondominiumUser_Users_UserId",
                table: "CondominiumUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationCode_Users_UserId",
                table: "EmailVerificationCode",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
