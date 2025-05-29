using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registry.Migrations
{
    /// <inheritdoc />
    public partial class AddedTradesManSpecialities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_TradesMan_TradesManId",
                table: "Specialties");

            migrationBuilder.DropIndex(
                name: "IX_Specialties_TradesManId",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "TradesManId",
                table: "Specialties");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "TradesMan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "TradesMan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TradesManSpecialities",
                columns: table => new
                {
                    TradesManId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradesManSpecialities", x => new { x.TradesManId, x.SpecialityId });
                    table.ForeignKey(
                        name: "FK_TradesManSpecialities_Specialties_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradesManSpecialities_TradesMan_TradesManId",
                        column: x => x.TradesManId,
                        principalTable: "TradesMan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradesManSpecialities_SpecialityId",
                table: "TradesManSpecialities",
                column: "SpecialityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradesManSpecialities");

            migrationBuilder.DropColumn(
                name: "City",
                table: "TradesMan");

            migrationBuilder.DropColumn(
                name: "County",
                table: "TradesMan");

            migrationBuilder.AddColumn<Guid>(
                name: "TradesManId",
                table: "Specialties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_TradesManId",
                table: "Specialties",
                column: "TradesManId");

            migrationBuilder.AddForeignKey(
                name: "FK_Specialties_TradesMan_TradesManId",
                table: "Specialties",
                column: "TradesManId",
                principalTable: "TradesMan",
                principalColumn: "Id");
        }
    }
}
