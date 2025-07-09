using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShift.Migrations
{
    /// <inheritdoc />
    public partial class AddJobRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssistantJob_Assistants_AssignedAssistantsAssistantId",
                table: "AssistantJob");

            migrationBuilder.DropForeignKey(
                name: "FK_AssistantJob_Jobs_AssignedJobsJobId",
                table: "AssistantJob");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Drivers_DriverId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssistantJob",
                table: "AssistantJob");

            migrationBuilder.RenameTable(
                name: "AssistantJob",
                newName: "JobAssistants");

            migrationBuilder.RenameIndex(
                name: "IX_AssistantJob_AssignedJobsJobId",
                table: "JobAssistants",
                newName: "IX_JobAssistants_AssignedJobsJobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobAssistants",
                table: "JobAssistants",
                columns: new[] { "AssignedAssistantsAssistantId", "AssignedJobsJobId" });

            migrationBuilder.AddForeignKey(
                name: "FK_JobAssistants_Assistants_AssignedAssistantsAssistantId",
                table: "JobAssistants",
                column: "AssignedAssistantsAssistantId",
                principalTable: "Assistants",
                principalColumn: "AssistantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobAssistants_Jobs_AssignedJobsJobId",
                table: "JobAssistants",
                column: "AssignedJobsJobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Drivers_DriverId",
                table: "Jobs",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobAssistants_Assistants_AssignedAssistantsAssistantId",
                table: "JobAssistants");

            migrationBuilder.DropForeignKey(
                name: "FK_JobAssistants_Jobs_AssignedJobsJobId",
                table: "JobAssistants");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Drivers_DriverId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobAssistants",
                table: "JobAssistants");

            migrationBuilder.RenameTable(
                name: "JobAssistants",
                newName: "AssistantJob");

            migrationBuilder.RenameIndex(
                name: "IX_JobAssistants_AssignedJobsJobId",
                table: "AssistantJob",
                newName: "IX_AssistantJob_AssignedJobsJobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssistantJob",
                table: "AssistantJob",
                columns: new[] { "AssignedAssistantsAssistantId", "AssignedJobsJobId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AssistantJob_Assistants_AssignedAssistantsAssistantId",
                table: "AssistantJob",
                column: "AssignedAssistantsAssistantId",
                principalTable: "Assistants",
                principalColumn: "AssistantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssistantJob_Jobs_AssignedJobsJobId",
                table: "AssistantJob",
                column: "AssignedJobsJobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Drivers_DriverId",
                table: "Jobs",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "VehicleId");
        }
    }
}
