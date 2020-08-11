using Microsoft.EntityFrameworkCore.Migrations;

namespace CL3C.Services.Migrations
{
    public partial class AddOwnerToCarsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Cars",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Cars");
        }
    }
}
