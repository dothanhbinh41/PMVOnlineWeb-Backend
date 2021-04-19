using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMVOnline.Migrations
{
    public partial class AddRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LeaderId",
                table: "AppTasks",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AppTaskRatings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    IsLeader = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Note = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExtraProperties = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTaskRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTaskRatings_AppAppUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AppAppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppTaskRatings_AppTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AppTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTasks_LeaderId",
                table: "AppTasks",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskRatings_CreatorId",
                table: "AppTaskRatings",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskRatings_TaskId",
                table: "AppTaskRatings",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_AppAppUsers_LeaderId",
                table: "AppTasks",
                column: "LeaderId",
                principalTable: "AppAppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_AppAppUsers_LeaderId",
                table: "AppTasks");

            migrationBuilder.DropTable(
                name: "AppTaskRatings");

            migrationBuilder.DropIndex(
                name: "IX_AppTasks_LeaderId",
                table: "AppTasks");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "AppTasks");
        }
    }
}
