using Microsoft.EntityFrameworkCore.Migrations;

namespace CL3C.Services.Migrations
{
    public partial class SeedCarsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "ID", "BaseCostsPerDistanceUnit", "BaseLifeCycleCosts", "FuelConsumption", "FuelPrice", "Name", "PurchasePrice", "TechnicalLife" },
                values: new object[] { 1ul, null, null, 7.5m, 30.0m, "Car A", 200000ul, 200000ul });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "ID", "BaseCostsPerDistanceUnit", "BaseLifeCycleCosts", "FuelConsumption", "FuelPrice", "Name", "PurchasePrice", "TechnicalLife" },
                values: new object[] { 2ul, null, null, 5.5m, 30.0m, "Car B", 150000ul, 150000ul });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 1ul);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 2ul);
        }
    }
}
