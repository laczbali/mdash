using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dashboard.API.Migrations
{
    public partial class AddingKanban : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KanbanProjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentUserId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KanbanProjects_Users_ParentUserId",
                        column: x => x.ParentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KanbanStories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentProjectId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KanbanStories_KanbanProjects_ParentProjectId",
                        column: x => x.ParentProjectId,
                        principalTable: "KanbanProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KanbanTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentStoryId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanbanTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KanbanTasks_KanbanStories_ParentStoryId",
                        column: x => x.ParentStoryId,
                        principalTable: "KanbanStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KanbanProjects_ParentUserId",
                table: "KanbanProjects",
                column: "ParentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanStories_ParentProjectId",
                table: "KanbanStories",
                column: "ParentProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_KanbanTasks_ParentStoryId",
                table: "KanbanTasks",
                column: "ParentStoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KanbanTasks");

            migrationBuilder.DropTable(
                name: "KanbanStories");

            migrationBuilder.DropTable(
                name: "KanbanProjects");
        }
    }
}
