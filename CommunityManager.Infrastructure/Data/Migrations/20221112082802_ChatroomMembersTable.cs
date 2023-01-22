using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityManager.Infrastructure.Data.Migrations
{
    public partial class ChatroomMembersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatroomsMembers",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatroomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatroomsMembers", x => new { x.ChatroomId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_ChatroomsMembers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatroomsMembers_Chatrooms_ChatroomId",
                        column: x => x.ChatroomId,
                        principalTable: "Chatrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatroomsMembers_ApplicationUserId",
                table: "ChatroomsMembers",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatroomsMembers");
        }
    }
}
