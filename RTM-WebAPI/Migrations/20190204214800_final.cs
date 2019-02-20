using Microsoft.EntityFrameworkCore.Migrations;

namespace RadonTestsManager.Migrations
{
    public partial class final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRMMaintenanceLogEntry_ContinuousRadonMonitors_CRMId1",
                table: "CRMMaintenanceLogEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CRMMaintenanceLogEntry",
                table: "CRMMaintenanceLogEntry");

            migrationBuilder.RenameTable(
                name: "CRMMaintenanceLogEntry",
                newName: "CRMMaintenanceLogs");

            migrationBuilder.RenameIndex(
                name: "IX_CRMMaintenanceLogEntry_CRMId1",
                table: "CRMMaintenanceLogs",
                newName: "IX_CRMMaintenanceLogs_CRMId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CRMMaintenanceLogs",
                table: "CRMMaintenanceLogs",
                column: "EntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRMMaintenanceLogs_ContinuousRadonMonitors_CRMId1",
                table: "CRMMaintenanceLogs",
                column: "CRMId1",
                principalTable: "ContinuousRadonMonitors",
                principalColumn: "CRMId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRMMaintenanceLogs_ContinuousRadonMonitors_CRMId1",
                table: "CRMMaintenanceLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CRMMaintenanceLogs",
                table: "CRMMaintenanceLogs");

            migrationBuilder.RenameTable(
                name: "CRMMaintenanceLogs",
                newName: "CRMMaintenanceLogEntry");

            migrationBuilder.RenameIndex(
                name: "IX_CRMMaintenanceLogs_CRMId1",
                table: "CRMMaintenanceLogEntry",
                newName: "IX_CRMMaintenanceLogEntry_CRMId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CRMMaintenanceLogEntry",
                table: "CRMMaintenanceLogEntry",
                column: "EntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRMMaintenanceLogEntry_ContinuousRadonMonitors_CRMId1",
                table: "CRMMaintenanceLogEntry",
                column: "CRMId1",
                principalTable: "ContinuousRadonMonitors",
                principalColumn: "CRMId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
