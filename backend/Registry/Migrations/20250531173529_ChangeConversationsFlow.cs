using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registry.Migrations
{
    /// <inheritdoc />
    public partial class ChangeConversationsFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequests_Jobs_ApprovedId",
                table: "ClientRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequests_Users_FromId",
                table: "ClientRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequests_Users_ToId",
                table: "ClientRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_User1Id",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_User2Id",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_ClientId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_TradesManId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_Jobs_JobId",
                table: "Specialties");

            migrationBuilder.DropIndex(
                name: "IX_Specialties_JobId",
                table: "Specialties");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ClientId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_ClientRequests_ApprovedId",
                table: "ClientRequests");

            migrationBuilder.DropIndex(
                name: "IX_ClientRequests_FromId",
                table: "ClientRequests");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "FromId",
                table: "ClientRequests");

            migrationBuilder.DropColumn(
                name: "WorkmanshipAmount",
                table: "ClientRequests");

            migrationBuilder.RenameColumn(
                name: "TradesManId",
                table: "Jobs",
                newName: "TradesManJobResponseId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_TradesManId",
                table: "Jobs",
                newName: "IX_Jobs_TradesManJobResponseId");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "Conversations",
                newName: "TradesManId");

            migrationBuilder.RenameColumn(
                name: "User1Id",
                table: "Conversations",
                newName: "RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_User2Id",
                table: "Conversations",
                newName: "IX_Conversations_TradesManId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_User1Id",
                table: "Conversations",
                newName: "IX_Conversations_RequestId");

            migrationBuilder.RenameColumn(
                name: "ToId",
                table: "ClientRequests",
                newName: "InitiatedById");

            migrationBuilder.RenameColumn(
                name: "AproximateEndDate",
                table: "ClientRequests",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "ApprovedId",
                table: "ClientRequests",
                newName: "JobApprovedId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientRequests_ToId",
                table: "ClientRequests",
                newName: "IX_ClientRequests_InitiatedById");

            migrationBuilder.AddColumn<bool>(
                name: "Declined",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Open",
                table: "ClientRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowToEveryone",
                table: "ClientRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ClientRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TradesManJobResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientJobRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TradesManId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkmanshipAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AproximationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Seen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradesManJobResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradesManJobResponses_ClientRequests_ClientJobRequestId",
                        column: x => x.ClientJobRequestId,
                        principalTable: "ClientRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradesManJobResponses_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TradesManJobResponses_Users_TradesManId",
                        column: x => x.TradesManId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientRequests_JobApprovedId",
                table: "ClientRequests",
                column: "JobApprovedId",
                unique: true,
                filter: "[JobApprovedId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TradesManJobResponses_ClientJobRequestId",
                table: "TradesManJobResponses",
                column: "ClientJobRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TradesManJobResponses_ConversationId",
                table: "TradesManJobResponses",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_TradesManJobResponses_TradesManId",
                table: "TradesManJobResponses",
                column: "TradesManId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequests_Jobs_JobApprovedId",
                table: "ClientRequests",
                column: "JobApprovedId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequests_Users_InitiatedById",
                table: "ClientRequests",
                column: "InitiatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ClientRequests_RequestId",
                table: "Conversations",
                column: "RequestId",
                principalTable: "ClientRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_TradesManId",
                table: "Conversations",
                column: "TradesManId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ClientRequests_Id",
                table: "Jobs",
                column: "Id",
                principalTable: "ClientRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_TradesManJobResponses_TradesManJobResponseId",
                table: "Jobs",
                column: "TradesManJobResponseId",
                principalTable: "TradesManJobResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequests_Jobs_JobApprovedId",
                table: "ClientRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequests_Users_InitiatedById",
                table: "ClientRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ClientRequests_RequestId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Users_TradesManId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ClientRequests_Id",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_TradesManJobResponses_TradesManJobResponseId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "TradesManJobResponses");

            migrationBuilder.DropIndex(
                name: "IX_ClientRequests_JobApprovedId",
                table: "ClientRequests");

            migrationBuilder.DropColumn(
                name: "Declined",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "ClientRequests");

            migrationBuilder.DropColumn(
                name: "ShowToEveryone",
                table: "ClientRequests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ClientRequests");

            migrationBuilder.RenameColumn(
                name: "TradesManJobResponseId",
                table: "Jobs",
                newName: "TradesManId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_TradesManJobResponseId",
                table: "Jobs",
                newName: "IX_Jobs_TradesManId");

            migrationBuilder.RenameColumn(
                name: "TradesManId",
                table: "Conversations",
                newName: "User2Id");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Conversations",
                newName: "User1Id");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_TradesManId",
                table: "Conversations",
                newName: "IX_Conversations_User2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_RequestId",
                table: "Conversations",
                newName: "IX_Conversations_User1Id");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "ClientRequests",
                newName: "AproximateEndDate");

            migrationBuilder.RenameColumn(
                name: "JobApprovedId",
                table: "ClientRequests",
                newName: "ApprovedId");

            migrationBuilder.RenameColumn(
                name: "InitiatedById",
                table: "ClientRequests",
                newName: "ToId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientRequests_InitiatedById",
                table: "ClientRequests",
                newName: "IX_ClientRequests_ToId");

            migrationBuilder.AddColumn<Guid>(
                name: "JobId",
                table: "Specialties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FromId",
                table: "ClientRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "WorkmanshipAmount",
                table: "ClientRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_JobId",
                table: "Specialties",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ClientId",
                table: "Jobs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRequests_ApprovedId",
                table: "ClientRequests",
                column: "ApprovedId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRequests_FromId",
                table: "ClientRequests",
                column: "FromId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequests_Jobs_ApprovedId",
                table: "ClientRequests",
                column: "ApprovedId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequests_Users_FromId",
                table: "ClientRequests",
                column: "FromId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequests_Users_ToId",
                table: "ClientRequests",
                column: "ToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_User1Id",
                table: "Conversations",
                column: "User1Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Users_User2Id",
                table: "Conversations",
                column: "User2Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_ClientId",
                table: "Jobs",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_TradesManId",
                table: "Jobs",
                column: "TradesManId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialties_Jobs_JobId",
                table: "Specialties",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");
        }
    }
}
