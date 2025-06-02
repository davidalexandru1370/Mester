using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registry.Migrations
{
    /// <inheritdoc />
    public partial class FixResponsesErrors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradesManJobResponses_ClientRequests_ClientJobRequestId",
                table: "TradesManJobResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_TradesManJobResponses_Conversations_ConversationId",
                table: "TradesManJobResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_TradesManJobResponses_Users_TradesManId",
                table: "TradesManJobResponses");

            migrationBuilder.DropIndex(
                name: "IX_TradesManJobResponses_ClientJobRequestId",
                table: "TradesManJobResponses");

            migrationBuilder.DropIndex(
                name: "IX_TradesManJobResponses_TradesManId",
                table: "TradesManJobResponses");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_RequestId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ClientJobRequestId",
                table: "TradesManJobResponses");

            migrationBuilder.DropColumn(
                name: "TradesManId",
                table: "TradesManJobResponses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "TradesManJobResponses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_RequestId_TradesManId",
                table: "Conversations",
                columns: new[] { "RequestId", "TradesManId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TradesManJobResponses_Conversations_ConversationId",
                table: "TradesManJobResponses",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradesManJobResponses_Conversations_ConversationId",
                table: "TradesManJobResponses");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_RequestId_TradesManId",
                table: "Conversations");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                table: "TradesManJobResponses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientJobRequestId",
                table: "TradesManJobResponses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TradesManId",
                table: "TradesManJobResponses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TradesManJobResponses_ClientJobRequestId",
                table: "TradesManJobResponses",
                column: "ClientJobRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TradesManJobResponses_TradesManId",
                table: "TradesManJobResponses",
                column: "TradesManId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_RequestId",
                table: "Conversations",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TradesManJobResponses_ClientRequests_ClientJobRequestId",
                table: "TradesManJobResponses",
                column: "ClientJobRequestId",
                principalTable: "ClientRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TradesManJobResponses_Conversations_ConversationId",
                table: "TradesManJobResponses",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TradesManJobResponses_Users_TradesManId",
                table: "TradesManJobResponses",
                column: "TradesManId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
