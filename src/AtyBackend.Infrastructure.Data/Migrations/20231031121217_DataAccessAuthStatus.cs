using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtyBackend.Infrastructure.Data.Migrations
{
    public partial class DataAccessAuthStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IsDataAuthorized",
                table: "WeatherStationUser",
                type: "int",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreator",
                table: "WeatherStationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCreator",
                table: "WeatherStationUser");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDataAuthorized",
                table: "WeatherStationUser",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
