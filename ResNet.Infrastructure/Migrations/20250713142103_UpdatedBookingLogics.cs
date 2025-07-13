using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBookingLogics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "TableId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookingCode",
                table: "Bookings",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Bookings",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Bookings",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                table: "Orders",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tables_TableId",
                table: "Orders",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Tables_TableId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TableId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TableId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BookingCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bookings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
