using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtyBackend.Infrastructure.Data.Migrations
{
    public partial class AccuracyDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Accuracy",
                table: "Sensor",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Accuracy",
                table: "Sensor",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
