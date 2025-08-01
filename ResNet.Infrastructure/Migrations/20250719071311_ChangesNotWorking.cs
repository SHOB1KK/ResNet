using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangesNotWorking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkingHour");

            migrationBuilder.DropColumn(
                name: "OwnerResumeUrl",
                table: "RestaurantRequests");

            migrationBuilder.DropColumn(
                name: "RestaurantImageUrl",
                table: "RestaurantRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerResumeUrl",
                table: "RestaurantRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantImageUrl",
                table: "RestaurantRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkingHour",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RestaurantRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Day = table.Column<int>(type: "INTEGER", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingHour", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkingHour_RestaurantRequests_RestaurantRequestId",
                        column: x => x.RestaurantRequestId,
                        principalTable: "RestaurantRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHour_RestaurantRequestId",
                table: "WorkingHour",
                column: "RestaurantRequestId");
        }
    }
}
