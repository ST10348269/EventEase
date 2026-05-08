using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class Part2AddEventImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add ImageUrl to Events
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Make Venue.ImageUrl nullable to allow venues without images
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Venues",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Create the vw_BookingDetails SQL view
            migrationBuilder.Sql(@"
                CREATE VIEW vw_BookingDetails AS
                SELECT
                    b.BookingID,
                    b.BookingDate,
                    e.EventName,
                    e.EventDate,
                    e.Description          AS EventDescription,
                    e.ImageUrl             AS EventImageUrl,
                    v.VenueName,
                    v.Location             AS VenueLocation,
                    v.Capacity             AS VenueCapacity,
                    v.ImageUrl             AS VenueImageUrl
                FROM
                    Bookings b
                    INNER JOIN Events  e ON b.EventID  = e.EventID
                    INNER JOIN Venues  v ON b.VenueID  = v.VenueID;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_BookingDetails;");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldNullable: true);
        }
    }
}
