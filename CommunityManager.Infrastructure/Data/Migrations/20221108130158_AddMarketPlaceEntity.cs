using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityManager.Infrastructure.Data.Migrations
{
    public partial class AddMarketPlaceEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MarketplaceId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Marketplaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marketplaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Marketplaces_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_MarketplaceId",
                table: "Products",
                column: "MarketplaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Marketplaces_CommunityId",
                table: "Marketplaces",
                column: "CommunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Marketplaces_MarketplaceId",
                table: "Products",
                column: "MarketplaceId",
                principalTable: "Marketplaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Marketplaces_MarketplaceId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Marketplaces");

            migrationBuilder.DropIndex(
                name: "IX_Products_MarketplaceId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MarketplaceId",
                table: "Products");
        }
    }
}
