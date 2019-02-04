using Microsoft.EntityFrameworkCore.Migrations;

namespace RadonTestsManager.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "LSVials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "CRMMaintenanceLogEntry",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "ContinuousRadonMonitors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "LSVials");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "CRMMaintenanceLogEntry");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "ContinuousRadonMonitors");
        }
    }
}
