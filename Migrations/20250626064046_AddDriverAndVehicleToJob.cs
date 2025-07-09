using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShift.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverAndVehicleToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_DriverId",
                table: "Jobs",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_VehicleId",
                table: "Jobs",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Drivers_DriverId",
                table: "Jobs",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Drivers_DriverId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_DriverId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_VehicleId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Jobs");
        }
    }
}
