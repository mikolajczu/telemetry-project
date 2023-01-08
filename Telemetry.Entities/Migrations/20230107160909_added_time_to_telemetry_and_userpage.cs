using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemetry.Entities.Migrations
{
    public partial class added_time_to_telemetry_and_userpage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Time",
                table: "UserPages",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Time",
                table: "TelemetrySessions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "UserPages");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "TelemetrySessions");
        }
    }
}
