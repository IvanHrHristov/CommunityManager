using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityManager.Infrastructure.Data.Migrations
{
    public partial class UpdateMarketplaceName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Marketplaces",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Marketplaces");
        }
    }
}
