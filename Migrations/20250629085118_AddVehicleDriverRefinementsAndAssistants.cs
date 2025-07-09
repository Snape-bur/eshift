using Microsoft.EntityFrameworkCore.Migrations;
using System; // Make sure this is included for System.Enum if needed, though not directly used in the migration

#nullable disable

namespace EShift.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleDriverRefinementsAndAssistants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop 'Availability' columns first
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Drivers");

            // Rename columns
            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Vehicles",
                newName: "CapacityCubicFeet");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Drivers",
                newName: "DriverId");

            // --- MANUAL STEPS FOR VEHICLES.TYPE (string to enum/int conversion) ---
            // 1. Add a temporary new column as int for the enum
            migrationBuilder.AddColumn<int>(
                name: "TypeTemp", // Temporary column name
                table: "Vehicles",
                type: "int",
                nullable: true); // Make it nullable temporarily to allow updates

            // 2. Migrate existing data from old string 'Type' to new int 'TypeTemp'
            // IMPORTANT: The integer values (0, 1, 2, etc.) must match the order
            // your VehicleType enum members are defined in Vehicle.cs
            // (e.g., Van=0, SmallTruck=1, MediumTruck=2, LargeTruck=3, PickupTruck=4, etc.)
            migrationBuilder.Sql(@"
                UPDATE Vehicles
                SET TypeTemp = CASE [Type]
                    WHEN 'Van' THEN 0
                    WHEN 'SmallTruck' THEN 1
                    WHEN 'MediumTruck' THEN 2
                    WHEN 'LargeTruck' THEN 3
                    WHEN 'PickupTruck' THEN 4
                    WHEN 'MotorCycle' THEN 5
                    WHEN 'FourDoorSedan' THEN 6
                    WHEN 'FiveDoorCar' THEN 7
                    WHEN 'MultiPurposeVehicle' THEN 8
                    WHEN 'SixWheelFusoTruck' THEN 9
                    ELSE 0 -- Default value for any unmapped or unexpected string values
                END;
            ");

            // 3. Drop the old string 'Type' column
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Vehicles");

            // 4. Rename the temporary int column to the original 'Type' name
            migrationBuilder.RenameColumn(
                name: "TypeTemp",
                table: "Vehicles",
                newName: "Type");

            // 5. Alter the column to be NOT NULL (as per your model's requirement)
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Vehicles",
                type: "int",
                nullable: false, // Matches [Required] in your C# model
                oldClrType: typeof(int), // This is not strictly necessary but keeps the old type consistent
                oldNullable: true); // Old column was temporarily nullable for update


            // --- MANUAL STEPS FOR VEHICLES.STATUS (string to enum/int conversion) ---
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Vehicles
                SET StatusTemp = CASE [Status]
                    WHEN 'Operational' THEN 0
                    WHEN 'OnJob' THEN 1
                    WHEN 'UnderMaintenance' THEN 2
                    WHEN 'BrokenDown' THEN 3
                    ELSE 0 -- Default
                END;
            ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Vehicles",
                newName: "Status");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Vehicles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            // --- MANUAL STEPS FOR DRIVERS.STATUS (string to enum/int conversion) ---
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Drivers",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Drivers
                SET StatusTemp = CASE [Status]
                    WHEN 'Available' THEN 0
                    WHEN 'OnJob' THEN 1
                    WHEN 'OffDuty' THEN 2
                    WHEN 'OnBreak' THEN 3
                    WHEN 'SickLeave' THEN 4
                    ELSE 0 -- Default
                END;
            ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Drivers",
                newName: "Status");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Drivers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);


            // --- New Column Additions (EF Core Generated - DO NOT CHANGE THESE) ---
            // These were correctly generated by EF Core for your new properties
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // This AlterColumn was also correctly generated by EF Core for LicenseNumber
            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true, // This should match your Driver.cs model's nullable status
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true); // This seems to be consistent, good.

            // --- Assistant Table Creation (EF Core Generated - DO NOT CHANGE THESE) ---
            // This entire block creates your new Assistant table and the join table (AssistantJob)
            migrationBuilder.CreateTable(
                name: "Assistants",
                columns: table => new
                {
                    AssistantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assistants", x => x.AssistantId);
                });

            migrationBuilder.CreateTable(
                name: "AssistantJob",
                columns: table => new
                {
                    AssignedAssistantsAssistantId = table.Column<int>(type: "int", nullable: false),
                    AssignedJobsJobId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistantJob", x => new { x.AssignedAssistantsAssistantId, x.AssignedJobsJobId });
                    table.ForeignKey(
                        name: "FK_AssistantJob_Assistants_AssignedAssistantsAssistantId",
                        column: x => x.AssignedAssistantsAssistantId,
                        principalTable: "Assistants",
                        principalColumn: "AssistantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssistantJob_Jobs_AssignedJobsJobId",
                        column: x => x.AssignedJobsJobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssistantJob_AssignedJobsJobId",
                table: "AssistantJob",
                column: "AssignedJobsJobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // You can leave the Down method as EF Core generated it.
            // Be aware that if you ever try to 'rollback' this migration after data is created
            // in the new integer columns, converting back to string might lead to issues.
            // For now, it's typically fine for a one-way update.

            migrationBuilder.DropTable(
                name: "AssistantJob");

            migrationBuilder.DropTable(
                name: "Assistants");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "CapacityCubicFeet",
                table: "Vehicles",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Drivers",
                newName: "Id");

            // These are the original AlterColumn methods from EF Core.
            // If you needed to revert the data for real, these would also need manual SQL for conversion.
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Availability",
                table: "Vehicles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Availability",
                table: "Drivers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}