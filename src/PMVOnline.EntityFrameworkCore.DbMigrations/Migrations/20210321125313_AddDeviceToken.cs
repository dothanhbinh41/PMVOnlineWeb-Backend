using Microsoft.EntityFrameworkCore.Migrations;

namespace PMVOnline.Migrations
{
    public partial class AddDeviceToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUserTokens_AbpUsers_UserId",
                table: "AbpUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AbpUserTokens",
                table: "AbpUserTokens");

            migrationBuilder.RenameTable(
                name: "AbpUserTokens",
                newName: "AppIdentityUserTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppIdentityUserTokens",
                table: "AppIdentityUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppIdentityUserTokens_AbpUsers_UserId",
                table: "AppIdentityUserTokens",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppIdentityUserTokens_AbpUsers_UserId",
                table: "AppIdentityUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppIdentityUserTokens",
                table: "AppIdentityUserTokens");

            migrationBuilder.RenameTable(
                name: "AppIdentityUserTokens",
                newName: "AbpUserTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbpUserTokens",
                table: "AbpUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUserTokens_AbpUsers_UserId",
                table: "AbpUserTokens",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
