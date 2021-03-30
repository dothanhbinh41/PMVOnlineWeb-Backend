using Microsoft.EntityFrameworkCore.Migrations;

namespace PMVOnline.Migrations
{
    public partial class UpdateReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppReferenceTasks_AppTasks_ReferenceTaskId",
                table: "AppReferenceTasks");

            migrationBuilder.DropIndex(
                name: "IX_AppReferenceTasks_ReferenceTaskId",
                table: "AppReferenceTasks");

            migrationBuilder.CreateIndex(
                name: "IX_AppReferenceTasks_TaskId",
                table: "AppReferenceTasks",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppReferenceTasks_AppTasks_TaskId",
                table: "AppReferenceTasks",
                column: "TaskId",
                principalTable: "AppTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppReferenceTasks_AppTasks_TaskId",
                table: "AppReferenceTasks");

            migrationBuilder.DropIndex(
                name: "IX_AppReferenceTasks_TaskId",
                table: "AppReferenceTasks");

            migrationBuilder.CreateIndex(
                name: "IX_AppReferenceTasks_ReferenceTaskId",
                table: "AppReferenceTasks",
                column: "ReferenceTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppReferenceTasks_AppTasks_ReferenceTaskId",
                table: "AppReferenceTasks",
                column: "ReferenceTaskId",
                principalTable: "AppTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
