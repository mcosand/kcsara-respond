using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kcsara.Respond.Data.Migrations.SqlServerMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "respond");

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "respond",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<long>(type: "bigint", nullable: false),
                    EndTime = table.Column<long>(type: "bigint", nullable: true),
                    Created = table.Column<long>(type: "bigint", nullable: false),
                    Updated = table.Column<long>(type: "bigint", nullable: false),
                    Updater = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RespondingUnits",
                schema: "respond",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requested = table.Column<long>(type: "bigint", nullable: false),
                    Activated = table.Column<long>(type: "bigint", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Updated = table.Column<long>(type: "bigint", nullable: false),
                    Updater = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespondingUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespondingUnits_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "respond",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RespondingUnits_ActivityId",
                schema: "respond",
                table: "RespondingUnits",
                column: "ActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespondingUnits",
                schema: "respond");

            migrationBuilder.DropTable(
                name: "Activities",
                schema: "respond");
        }
    }
}
