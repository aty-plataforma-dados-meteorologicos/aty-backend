using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtyBackend.Infrastructure.Data.Migrations
{
    public partial class AddPartnerIsPublicData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublicData",
                table: "Partners",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublicData",
                table: "Partners");
        }
    }
}
