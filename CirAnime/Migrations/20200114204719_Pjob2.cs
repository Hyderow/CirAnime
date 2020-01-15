using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CirAnime.Migrations
{
    public partial class Pjob2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(nullable: true),
                    duration = table.Column<int>(nullable: false),
                    live = table.Column<bool>(nullable: false),
                    thumbnail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaInfo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordID = table.Column<ulong>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    url = table.Column<string>(nullable: true),
                    contentType = table.Column<string>(nullable: true),
                    quality = table.Column<int>(nullable: false),
                    bitrate = table.Column<int>(nullable: false),
                    MediaInfoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Source_MediaInfo_MediaInfoID",
                        column: x => x.MediaInfoID,
                        principalTable: "MediaInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TextTrack",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    url = table.Column<string>(nullable: true),
                    contentType = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    MediaInfoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextTrack", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TextTrack_MediaInfo_MediaInfoID",
                        column: x => x.MediaInfoID,
                        principalTable: "MediaInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UploadEntry",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    OriginalFileName = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    MediaInfoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadEntry", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UploadEntry_MediaInfo_MediaInfoID",
                        column: x => x.MediaInfoID,
                        principalTable: "MediaInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UploadEntry_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingJob",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginalFile = table.Column<string>(nullable: true),
                    UploadEntryID = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Progress = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingJob", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProcessingJob_UploadEntry_UploadEntryID",
                        column: x => x.UploadEntryID,
                        principalTable: "UploadEntry",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJob_UploadEntryID",
                table: "ProcessingJob",
                column: "UploadEntryID");

            migrationBuilder.CreateIndex(
                name: "IX_Source_MediaInfoID",
                table: "Source",
                column: "MediaInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_TextTrack_MediaInfoID",
                table: "TextTrack",
                column: "MediaInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_UploadEntry_MediaInfoID",
                table: "UploadEntry",
                column: "MediaInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_UploadEntry_UserID",
                table: "UploadEntry",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessingJob");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropTable(
                name: "TextTrack");

            migrationBuilder.DropTable(
                name: "UploadEntry");

            migrationBuilder.DropTable(
                name: "MediaInfo");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
