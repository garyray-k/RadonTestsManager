using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadonTestsManager.Migrations
{
    public partial class updatedJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRMMaintenanceLogs_ContinuousRadonMonitors_CRMId1",
                table: "CRMMaintenanceLogs");

            migrationBuilder.RenameColumn(
                name: "CRMId1",
                table: "CRMMaintenanceLogs",
                newName: "CRMId");

            migrationBuilder.RenameIndex(
                name: "IX_CRMMaintenanceLogs_CRMId1",
                table: "CRMMaintenanceLogs",
                newName: "IX_CRMMaintenanceLogs_CRMId");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceDate",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TimeOfDay",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CRMMaintenanceLogs_ContinuousRadonMonitors_CRMId",
                table: "CRMMaintenanceLogs",
                column: "CRMId",
                principalTable: "ContinuousRadonMonitors",
                principalColumn: "CRMId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRMMaintenanceLogs_ContinuousRadonMonitors_CRMId",
                table: "CRMMaintenanceLogs");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ServiceDate",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TimeOfDay",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "CRMId",
                table: "CRMMaintenanceLogs",
                newName: "CRMId1");

            migrationBuilder.RenameIndex(
                name: "IX_CRMMaintenanceLogs_CRMId",
                table: "CRMMaintenanceLogs",
                newName: "IX_CRMMaintenanceLogs_CRMId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CRMMaintenanceLogs_ContinuousRadonMonitors_CRMId1",
                table: "CRMMaintenanceLogs",
                column: "CRMId1",
                principalTable: "ContinuousRadonMonitors",
                principalColumn: "CRMId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
