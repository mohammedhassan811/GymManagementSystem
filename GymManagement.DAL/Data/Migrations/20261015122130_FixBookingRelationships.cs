using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Sessions_sessionId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_sessionId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "sessionId",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Sessions_SessionsId",
                table: "Bookings",
                column: "SessionsId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Sessions_SessionsId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "sessionId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_sessionId",
                table: "Bookings",
                column: "sessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Sessions_sessionId",
                table: "Bookings",
                column: "sessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
