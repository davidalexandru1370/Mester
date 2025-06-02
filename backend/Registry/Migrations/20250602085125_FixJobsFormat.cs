using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registry.Migrations
{
    /// <inheritdoc />
    public partial class FixJobsFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ClientRequests_Id",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_TradesManJobResponseId",
                table: "Jobs");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TradesManJobResponseId",
                table: "Jobs",
                column: "TradesManJobResponseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jobs_TradesManJobResponseId",
                table: "Jobs");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_TradesManJobResponseId",
                table: "Jobs",
                column: "TradesManJobResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ClientRequests_Id",
                table: "Jobs",
                column: "Id",
                principalTable: "ClientRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
