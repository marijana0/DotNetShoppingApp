using Microsoft.EntityFrameworkCore.Migrations;

namespace DotNetShop.Migrations
{
    public partial class AddApplicationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Types",
                table: "Types");

            migrationBuilder.RenameTable(
                name: "Types",
                newName: "ApplicationTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationTypes",
                table: "ApplicationTypes",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationTypes",
                table: "ApplicationTypes");

            migrationBuilder.RenameTable(
                name: "ApplicationTypes",
                newName: "Types");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Types",
                table: "Types",
                column: "Id");
        }
    }
}
