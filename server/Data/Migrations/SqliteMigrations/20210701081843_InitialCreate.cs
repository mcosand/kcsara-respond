using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kcsara.Respond.Data.Migrations.SqliteMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    StartTime = table.Column<long>(type: "INTEGER", nullable: false),
                    EndTime = table.Column<long>(type: "INTEGER", nullable: true),
                    Created = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated = table.Column<long>(type: "INTEGER", nullable: false),
                    Updater = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RespondingUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Requested = table.Column<long>(type: "INTEGER", nullable: false),
                    Activated = table.Column<long>(type: "INTEGER", nullable: true),
                    ActivityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Updated = table.Column<long>(type: "INTEGER", nullable: false),
                    Updater = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespondingUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespondingUnits_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RespondingUnits_ActivityId",
                table: "RespondingUnits",
                column: "ActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespondingUnits");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
