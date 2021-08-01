using Microsoft.EntityFrameworkCore.Migrations;

namespace Medius.DataAccess.Migrations
{
    public partial class removeCaseRelationFromstripe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripePayments_Cases_CaseId",
                table: "StripePayments");

            migrationBuilder.DropIndex(
                name: "IX_StripePayments_CaseId",
                table: "StripePayments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StripePayments_CaseId",
                table: "StripePayments",
                column: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripePayments_Cases_CaseId",
                table: "StripePayments",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
