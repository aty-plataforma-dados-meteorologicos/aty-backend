using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtyBackend.Infrastructure.Data.Migrations
{
    public partial class FixingPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partner_WeatherStation_WeatherStationId",
                table: "Partner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Partner",
                table: "Partner");

            migrationBuilder.RenameTable(
                name: "Partner",
                newName: "Partners");

            migrationBuilder.RenameIndex(
                name: "IX_Partner_WeatherStationId",
                table: "Partners",
                newName: "IX_Partners_WeatherStationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Partners",
                table: "Partners",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_WeatherStation_WeatherStationId",
                table: "Partners",
                column: "WeatherStationId",
                principalTable: "WeatherStation",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_WeatherStation_WeatherStationId",
                table: "Partners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Partners",
                table: "Partners");

            migrationBuilder.RenameTable(
                name: "Partners",
                newName: "Partner");

            migrationBuilder.RenameIndex(
                name: "IX_Partners_WeatherStationId",
                table: "Partner",
                newName: "IX_Partner_WeatherStationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Partner",
                table: "Partner",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_WeatherStation_WeatherStationId",
                table: "Partner",
                column: "WeatherStationId",
                principalTable: "WeatherStation",
                principalColumn: "Id");
        }
    }
}
