using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShift.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedKgToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EstimatedKg",
                table: "Jobs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedKg",
                table: "Jobs");
        }
    }
}
