using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesSysytem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsChangedfiledInLeaveApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChanged",
                table: "LeaveApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChanged",
                table: "LeaveApplications");
        }
    }
}
