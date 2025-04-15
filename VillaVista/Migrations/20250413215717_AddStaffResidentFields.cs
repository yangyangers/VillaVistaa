using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VillaVista.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffResidentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MoveInDate",
                table: "Homeowners",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Homeowners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Homeowners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoveInDate",
                table: "Homeowners");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Homeowners");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Homeowners");
        }
    }
}
