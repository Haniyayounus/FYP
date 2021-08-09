using Microsoft.EntityFrameworkCore.Migrations;

namespace Medius.DataAccess.Migrations
{
    public partial class adduseridFromstripe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StripePayments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePayments_UserId",
                table: "StripePayments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripePayments_AspNetUsers_UserId",
                table: "StripePayments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripePayments_AspNetUsers_UserId",
                table: "StripePayments");

            migrationBuilder.DropIndex(
                name: "IX_StripePayments_UserId",
                table: "StripePayments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StripePayments");
        }
    }
}
