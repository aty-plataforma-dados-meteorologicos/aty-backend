using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtyBackend.Infrastructure.Data.Migrations
{
    public partial class AddWeatherStationIdOnPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_WeatherStation_WeatherStationId",
                table: "Partners");

            migrationBuilder.AlterColumn<int>(
                name: "WeatherStationId",
                table: "Partners",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_WeatherStation_WeatherStationId",
                table: "Partners",
                column: "WeatherStationId",
                principalTable: "WeatherStation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_WeatherStation_WeatherStationId",
                table: "Partners");

            migrationBuilder.AlterColumn<int>(
                name: "WeatherStationId",
                table: "Partners",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_WeatherStation_WeatherStationId",
                table: "Partners",
                column: "WeatherStationId",
                principalTable: "WeatherStation",
                principalColumn: "Id");
        }
    }
}
