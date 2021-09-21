using Microsoft.EntityFrameworkCore.Migrations;

namespace Medius.DataAccess.Migrations
{
    public partial class AddCopyrightPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CopyrightPlan",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CopyrightPlan",
                table: "Cases");
        }
    }
}
