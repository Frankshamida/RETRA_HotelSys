using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RETRA_HotelSys.Migrations
{
    /// <inheritdoc />
    public partial class FixGuestEmailType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "GuestReservations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                table: "GuestReservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuestPhone",
                table: "GuestReservations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "GuestReservations");

            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "GuestReservations");

            migrationBuilder.DropColumn(
                name: "GuestPhone",
                table: "GuestReservations");
        }
    }
}
