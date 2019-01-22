using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadonTestsManager.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerName = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "LSVials",
                columns: table => new
                {
                    LSVialId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SerialNumber = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    TestStart = table.Column<DateTime>(nullable: false),
                    TestFinish = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LSVials", x => x.LSVialId);
                });

            migrationBuilder.CreateTable(
                name: "ContinuousRadonMonitors",
                columns: table => new
                {
                    CRMId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MonitorNumber = table.Column<int>(nullable: false),
                    SerialNumber = table.Column<int>(nullable: false),
                    LastCalibrationDate = table.Column<DateTime>(nullable: false),
                    PurchaseDate = table.Column<DateTime>(nullable: false),
                    LastBatteryChangeDate = table.Column<DateTime>(nullable: false),
                    TestStart = table.Column<DateTime>(nullable: false),
                    TestFinish = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    LocationAddressId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContinuousRadonMonitors", x => x.CRMId);
                    table.ForeignKey(
                        name: "FK_ContinuousRadonMonitors_Addresses_LocationAddressId",
                        column: x => x.LocationAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CRMMaintenanceLogEntry",
                columns: table => new
                {
                    EntryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CRMId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMMaintenanceLogEntry", x => x.EntryId);
                    table.ForeignKey(
                        name: "FK_CRMMaintenanceLogEntry_ContinuousRadonMonitors_CRMId1",
                        column: x => x.CRMId1,
                        principalTable: "ContinuousRadonMonitors",
                        principalColumn: "CRMId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    JobId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceType = table.Column<string>(nullable: true),
                    ServiceDeadLine = table.Column<DateTime>(nullable: false),
                    DeviceType = table.Column<string>(nullable: true),
                    AccessInfo = table.Column<string>(nullable: true),
                    SpecialNotes = table.Column<string>(nullable: true),
                    Driver = table.Column<string>(nullable: true),
                    ArrivalTime = table.Column<DateTime>(nullable: false),
                    ContinousRadonMonitorCRMId = table.Column<int>(nullable: true),
                    LSVialId = table.Column<int>(nullable: true),
                    JobNumber = table.Column<int>(nullable: true),
                    JobAddressAddressId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_Jobs_ContinuousRadonMonitors_ContinousRadonMonitorCRMId",
                        column: x => x.ContinousRadonMonitorCRMId,
                        principalTable: "ContinuousRadonMonitors",
                        principalColumn: "CRMId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_Addresses_JobAddressAddressId",
                        column: x => x.JobAddressAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_LSVials_LSVialId",
                        column: x => x.LSVialId,
                        principalTable: "LSVials",
                        principalColumn: "LSVialId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContinuousRadonMonitors_LocationAddressId",
                table: "ContinuousRadonMonitors",
                column: "LocationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_CRMMaintenanceLogEntry_CRMId1",
                table: "CRMMaintenanceLogEntry",
                column: "CRMId1");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ContinousRadonMonitorCRMId",
                table: "Jobs",
                column: "ContinousRadonMonitorCRMId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobAddressAddressId",
                table: "Jobs",
                column: "JobAddressAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_LSVialId",
                table: "Jobs",
                column: "LSVialId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRMMaintenanceLogEntry");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "ContinuousRadonMonitors");

            migrationBuilder.DropTable(
                name: "LSVials");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
