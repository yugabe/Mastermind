using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mastermind.Api.Data.Migrations
{
    public partial class InitialMastermindSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<Guid>(nullable: false),
                    GameStarted = table.Column<DateTimeOffset>(nullable: false),
                    KeyLength = table.Column<int>(nullable: false),
                    DurationInSeconds = table.Column<double>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    GuessesMade = table.Column<int>(nullable: false),
                    PossibleValues = table.Column<int>(nullable: false),
                    MaximumPossibleGuesses = table.Column<int>(nullable: false),
                    AllowDuplicates = table.Column<bool>(nullable: false),
                    Won = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_Users_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scores_PlayerId",
                table: "Scores",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_Won",
                table: "Scores",
                column: "Won");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_KeyLength_PossibleValues_MaximumPossibleGuesses_GuessesMade_DurationInSeconds",
                table: "Scores",
                columns: new[] { "KeyLength", "PossibleValues", "MaximumPossibleGuesses", "GuessesMade", "DurationInSeconds" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
