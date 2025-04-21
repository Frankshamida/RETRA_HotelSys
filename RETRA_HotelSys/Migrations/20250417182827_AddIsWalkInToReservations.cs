using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RETRA_HotelSys.Migrations
{
    /// <inheritdoc />
    public partial class AddIsWalkInToReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWalkIn",
                table: "GuestReservations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWalkIn",
                table: "GuestReservations");
        }
    }
}
