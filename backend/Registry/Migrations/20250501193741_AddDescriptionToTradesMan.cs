using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registry.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToTradesMan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TradesMan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TradesMan");
        }
    }
}
