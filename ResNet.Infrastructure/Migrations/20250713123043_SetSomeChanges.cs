using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetSomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Restaurants_RestaurantId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCategory_Categories_CategoryId",
                table: "RestaurantCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCategory_Restaurants_RestaurantId",
                table: "RestaurantCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantCategory",
                table: "RestaurantCategory");

            migrationBuilder.RenameTable(
                name: "RestaurantCategory",
                newName: "RestaurantCategories");

            migrationBuilder.RenameIndex(
                name: "IX_RestaurantCategory_CategoryId",
                table: "RestaurantCategories",
                newName: "IX_RestaurantCategories_CategoryId");

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "Categories",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantCategories",
                table: "RestaurantCategories",
                columns: new[] { "RestaurantId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Restaurants_RestaurantId",
                table: "Categories",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCategories_Categories_CategoryId",
                table: "RestaurantCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCategories_Restaurants_RestaurantId",
                table: "RestaurantCategories",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Restaurants_RestaurantId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCategories_Categories_CategoryId",
                table: "RestaurantCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCategories_Restaurants_RestaurantId",
                table: "RestaurantCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantCategories",
                table: "RestaurantCategories");

            migrationBuilder.RenameTable(
                name: "RestaurantCategories",
                newName: "RestaurantCategory");

            migrationBuilder.RenameIndex(
                name: "IX_RestaurantCategories_CategoryId",
                table: "RestaurantCategory",
                newName: "IX_RestaurantCategory_CategoryId");

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantCategory",
                table: "RestaurantCategory",
                columns: new[] { "RestaurantId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Restaurants_RestaurantId",
                table: "Categories",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCategory_Categories_CategoryId",
                table: "RestaurantCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCategory_Restaurants_RestaurantId",
                table: "RestaurantCategory",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
