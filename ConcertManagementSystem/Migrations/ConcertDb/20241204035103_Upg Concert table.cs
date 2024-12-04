using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Website.Migrations.ConcertDb
{
    /// <inheritdoc />
    public partial class UpgConcerttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Concerts_Venues_VenueId",
                table: "Concerts");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "Concerts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Concerts_Venues_VenueId",
                table: "Concerts",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Concerts_Venues_VenueId",
                table: "Concerts");

            migrationBuilder.AlterColumn<int>(
                name: "VenueId",
                table: "Concerts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Concerts_Venues_VenueId",
                table: "Concerts",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
