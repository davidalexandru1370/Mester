using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registry.Migrations
{
    /// <inheritdoc />
    public partial class WithSpecialitiesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_TradesMan_TradesManProfileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TradesManProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TradesManProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TradesMan");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Specialties",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_Type",
                table: "Specialties",
                column: "Type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TradesMan_Users_Id",
                table: "TradesMan",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradesMan_Users_Id",
                table: "TradesMan");

            migrationBuilder.DropIndex(
                name: "IX_Specialties_Type",
                table: "Specialties");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "Username");

            migrationBuilder.AddColumn<Guid>(
                name: "TradesManProfileId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TradesMan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Specialties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TradesManProfileId",
                table: "Users",
                column: "TradesManProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TradesMan_TradesManProfileId",
                table: "Users",
                column: "TradesManProfileId",
                principalTable: "TradesMan",
                principalColumn: "Id");
        }
    }
}
