using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VillaVista.Migrations
{
    /// <inheritdoc />
    public partial class SpecifyAmountDuePrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Street",
                table: "Homeowners");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Homeowners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
