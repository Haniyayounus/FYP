using Microsoft.EntityFrameworkCore.Migrations;

namespace Medius.DataAccess.Migrations
{
    public partial class CaseRelationFromstripe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StripePayments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripePayments_CaseId",
                table: "StripePayments",
                column: "CaseId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_StripePayments_Cases_CaseId",
                table: "StripePayments",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripePayments_AspNetUsers_UserId",
                table: "StripePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_StripePayments_Cases_CaseId",
                table: "StripePayments");

            migrationBuilder.DropIndex(
                name: "IX_StripePayments_CaseId",
                table: "StripePayments");

            migrationBuilder.DropIndex(
                name: "IX_StripePayments_UserId",
                table: "StripePayments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StripePayments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
