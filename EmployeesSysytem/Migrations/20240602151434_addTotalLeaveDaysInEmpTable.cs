using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesSysytem.Migrations
{
    /// <inheritdoc />
    public partial class addTotalLeaveDaysInEmpTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalLeaveDays",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalLeaveDays",
                table: "Employees");
        }
    }
}
