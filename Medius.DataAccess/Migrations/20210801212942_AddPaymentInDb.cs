using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medius.DataAccess.Migrations
{
    public partial class AddPaymentInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Cities_CityId",
                table: "Cases");

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

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "StripePayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModify = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripePayments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StripePayments_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripePayments_CaseId",
                table: "StripePayments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_StripePayments_UserId",
                table: "StripePayments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Cities_CityId",
                table: "Cases",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Claims_ClaimId",
                table: "Cases",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Cities_CityId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Claims_ClaimId",
                table: "Cases");

            migrationBuilder.DropTable(
                name: "StripePayments");

            migrationBuilder.AlterColumn<int>(
                name: "ClaimId",
                table: "Cases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Cases",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Cities_CityId",
                table: "Cases",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Claims_ClaimId",
                table: "Cases",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
