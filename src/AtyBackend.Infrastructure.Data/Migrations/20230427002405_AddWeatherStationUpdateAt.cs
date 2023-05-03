using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtyBackend.Infrastructure.Data.Migrations
{
    public partial class AddWeatherStationUpdateAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "WeatherStation",
                newName: "UpdateAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "WeatherStation",
                newName: "ModifiedAt");
        }
    }
}
