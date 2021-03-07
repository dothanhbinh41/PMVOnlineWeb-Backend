using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMVOnline.Migrations
{
    public partial class AddTaskNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTaskActions");

            migrationBuilder.CreateTable(
                name: "AppTaskHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TaskId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActorId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppTaskHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTaskHistories_AppTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AppTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppTaskNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TaskId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ActorId = table.Column<Guid>(type: "char(36)", nullable: true),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppTaskNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTaskNotifications_AppTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AppTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskHistories_TaskId",
                table: "AppTaskHistories",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskNotifications_TaskId",
                table: "AppTaskNotifications",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTaskHistories");

            migrationBuilder.DropTable(
                name: "AppTaskNotifications");

            migrationBuilder.CreateTable(
                name: "AppTaskActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<Guid>(type: "char(36)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40) CHARACTER SET utf8mb4", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExtraProperties = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true),
                    Note = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TaskId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTaskActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTaskActions_AppTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AppTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskActions_TaskId",
                table: "AppTaskActions",
                column: "TaskId");
        }
    }
}
