using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityManager.Infrastructure.Data.Migrations
{
    public partial class AddPhotoToCommunity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Communities",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "PhotoLenght",
                table: "Communities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "PhotoLenght",
                table: "Communities");
        }
    }
}
