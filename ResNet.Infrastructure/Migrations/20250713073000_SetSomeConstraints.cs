using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetSomeConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Restaurants",
                type: "TEXT",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Restaurants");
        }
    }
}
