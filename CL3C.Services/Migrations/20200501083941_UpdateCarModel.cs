using Microsoft.EntityFrameworkCore.Migrations;

namespace CL3C.Services.Migrations
{
    public partial class UpdateCarModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseLifeCycleCosts",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "BaseCostsPerDistanceUnit",
                table: "Cars");

            migrationBuilder.AddColumn<decimal>(
                name: "BaseLifeCycleCosts",
                table: "Cars",
                type: "decimal(65,30)",
                nullable: false);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseCostsPerDistanceUnit",
                table: "Cars",
                type: "decimal(65,30)",
                nullable: false);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 1ul,
                columns: new[] { "BaseCostsPerDistanceUnit", "BaseLifeCycleCosts" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 2ul,
                columns: new[] { "BaseCostsPerDistanceUnit", "BaseLifeCycleCosts" },
                values: new object[] { 0m, 0m });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AlterColumn<decimal>(
            //     name: "BaseLifeCycleCosts",
            //     table: "Cars",
            //     type: "decimal(65,30)",
            //     nullable: true,
            //     oldClrType: typeof(decimal));

            // migrationBuilder.AlterColumn<decimal>(
            //     name: "BaseCostsPerDistanceUnit",
            //     table: "Cars",
            //     type: "decimal(65,30)",
            //     nullable: true,
            //     oldClrType: typeof(decimal));

            migrationBuilder.DropColumn(
                name: "BaseLifeCycleCosts",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "BaseCostsPerDistanceUnit",
                table: "Cars");

            migrationBuilder.AddColumn<decimal>(
                name: "BaseLifeCycleCosts",
                table: "Cars",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseCostsPerDistanceUnit",
                table: "Cars",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 1ul,
                columns: new[] { "BaseCostsPerDistanceUnit", "BaseLifeCycleCosts" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 2ul,
                columns: new[] { "BaseCostsPerDistanceUnit", "BaseLifeCycleCosts" },
                values: new object[] { null, null });
        }
    }
}
