using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtyBackend.Infrastructure.Data.Migrations
{
    public partial class AddWeatherStationPublicId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "WeatherStation",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "WeatherStation");
        }
    }
}
