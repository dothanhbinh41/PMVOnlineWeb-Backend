using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMVOnline.Migrations
{
    public partial class NotNullForLeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_AppAppUsers_LeaderId",
                table: "AppTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "LeaderId",
                table: "AppTasks",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_AppAppUsers_LeaderId",
                table: "AppTasks",
                column: "LeaderId",
                principalTable: "AppAppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_AppAppUsers_LeaderId",
                table: "AppTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "LeaderId",
                table: "AppTasks",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_AppAppUsers_LeaderId",
                table: "AppTasks",
                column: "LeaderId",
                principalTable: "AppAppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
