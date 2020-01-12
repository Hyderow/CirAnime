using Microsoft.EntityFrameworkCore.Migrations;

namespace CirAnime.Migrations
{
    public partial class Uploader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "UploadEntry",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "UploadEntry",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "UploadEntry");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "UploadEntry");
        }
    }
}
