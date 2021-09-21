using Microsoft.EntityFrameworkCore.Migrations;

namespace Medius.DataAccess.Migrations
{
    public partial class MakeClaimIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Claims_ClaimId",
                table: "Cases");

            migrationBuilder.AlterColumn<int>(
                name: "ClaimId",
                table: "Cases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Claims_ClaimId",
                table: "Cases",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Claims_ClaimId",
                table: "Cases");

            migrationBuilder.AlterColumn<int>(
                name: "ClaimId",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Claims_ClaimId",
                table: "Cases",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
