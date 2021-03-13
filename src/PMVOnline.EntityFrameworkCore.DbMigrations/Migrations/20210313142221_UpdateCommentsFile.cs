using Microsoft.EntityFrameworkCore.Migrations;

namespace PMVOnline.Migrations
{
    public partial class UpdateCommentsFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskCommentFiles_AppFiles_FileId",
                table: "AppTaskCommentFiles");

            migrationBuilder.DropIndex(
                name: "IX_AppTaskCommentFiles_FileId",
                table: "AppTaskCommentFiles");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "AppTaskCommentFiles",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "AppTaskCommentFiles",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "AppTaskCommentFiles");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "AppTaskCommentFiles");

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskCommentFiles_FileId",
                table: "AppTaskCommentFiles",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskCommentFiles_AppFiles_FileId",
                table: "AppTaskCommentFiles",
                column: "FileId",
                principalTable: "AppFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
