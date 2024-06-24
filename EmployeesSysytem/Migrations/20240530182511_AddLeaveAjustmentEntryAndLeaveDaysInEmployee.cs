using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesSysytem.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveAjustmentEntryAndLeaveDaysInEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AllocatedLeaveDays",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LeaveOutStandingBalance",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllocatedLeaveDays",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LeaveOutStandingBalance",
                table: "Employees");
        }
    }
}
