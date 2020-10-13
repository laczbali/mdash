using Microsoft.EntityFrameworkCore.Migrations;

namespace Dashboard.API.Migrations
{
    public partial class UpdatingSettings2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "SettingFields",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "SettingFields",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "SettingFields",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "SettingFields",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "SettingFields",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SettingFields",
                newName: "name");
        }
    }
}
