using Microsoft.EntityFrameworkCore.Migrations;

namespace Kcsara.Respond.Data.Migrations.SqlServerMigrations
{
    public partial class WellKnownUnits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KnownUnitId",
                schema: "respond",
                table: "RespondingUnits",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KnownUnitId",
                schema: "respond",
                table: "RespondingUnits");
        }
    }
}
