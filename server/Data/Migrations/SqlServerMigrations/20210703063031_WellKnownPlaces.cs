using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace Kcsara.Respond.Data.Migrations.SqlServerMigrations
{
    public partial class WellKnownPlaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Geometry>(
                name: "Location_Geometry",
                schema: "respond",
                table: "Activities",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_Name",
                schema: "respond",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_PropertiesJson",
                schema: "respond",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location_Wkid",
                schema: "respond",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location_Geometry",
                schema: "respond",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Location_Name",
                schema: "respond",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Location_PropertiesJson",
                schema: "respond",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Location_Wkid",
                schema: "respond",
                table: "Activities");
        }
    }
}
