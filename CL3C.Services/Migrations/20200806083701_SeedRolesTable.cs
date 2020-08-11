using Microsoft.EntityFrameworkCore.Migrations;

namespace CL3C.Services.Migrations
{
    public partial class SeedRolesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cars",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20) CHARACTER SET utf8mb4",
                oldMaxLength: 20);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2c6924b5-bb2b-4f69-9775-3d04e76a8051", "3abe21ec-80a3-457e-936d-ed44898df13d", "SuperAdmin", "SUPERADMIN" },
                    { "7abbc7b2-dc50-432a-b983-2eac6553094a", "755ab23a-b7cf-4108-8e7d-4f0414c9abb6", "Admin", "ADMIN" },
                    { "015c2d4b-3a02-4404-ba7a-a94e6a14ab7d", "eb06bd5e-146d-4bf6-9aea-bb526cc80f1d", "User", "USER" }
                });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 1ul,
                column: "Owner",
                value: "nomis");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "ID",
                keyValue: 2ul,
                column: "Owner",
                value: "nomis");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "015c2d4b-3a02-4404-ba7a-a94e6a14ab7d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c6924b5-bb2b-4f69-9775-3d04e76a8051");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7abbc7b2-dc50-432a-b983-2eac6553094a");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cars",
                type: "varchar(20) CHARACTER SET utf8mb4",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 40);
        }
    }
}
