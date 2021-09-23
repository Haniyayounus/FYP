using Microsoft.EntityFrameworkCore.Migrations;

namespace Medius.DataAccess.Migrations
{
    public partial class RoleInNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Notifications");
        }
    }
}
