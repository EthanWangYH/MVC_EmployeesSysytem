using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesSysytem.Migrations
{
    /// <inheritdoc />
    public partial class ADDDisabilityInEmp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisabilityId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabilityId",
                table: "Employees");
        }
    }
}
