using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemetry.Entities.Migrations
{
    public partial class added_status_to_telemetry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "TelemetrySessions",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TelemetrySessions");
        }
    }
}
