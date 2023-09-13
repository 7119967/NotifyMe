using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotifyMe.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPropCurrentThresholdinEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentThreshold",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "CurrentThreshold",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentThreshold",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "CurrentThreshold",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
