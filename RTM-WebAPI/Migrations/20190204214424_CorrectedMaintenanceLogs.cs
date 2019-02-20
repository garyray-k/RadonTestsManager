using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadonTestsManager.Migrations
{
    public partial class CorrectedMaintenanceLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionsTaken",
                table: "CRMMaintenanceLogEntry",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EntryDate",
                table: "CRMMaintenanceLogEntry",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceDescription",
                table: "CRMMaintenanceLogEntry",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionsTaken",
                table: "CRMMaintenanceLogEntry");

            migrationBuilder.DropColumn(
                name: "EntryDate",
                table: "CRMMaintenanceLogEntry");

            migrationBuilder.DropColumn(
                name: "MaintenanceDescription",
                table: "CRMMaintenanceLogEntry");
        }
    }
}
