using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSearchWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Jobseekers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobseekers_AppUserId",
                table: "Jobseekers",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AppUserId",
                table: "Companies",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AspNetUsers_AppUserId",
                table: "Companies",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobseekers_AspNetUsers_AppUserId",
                table: "Jobseekers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AspNetUsers_AppUserId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobseekers_AspNetUsers_AppUserId",
                table: "Jobseekers");

            migrationBuilder.DropIndex(
                name: "IX_Jobseekers_AppUserId",
                table: "Jobseekers");

            migrationBuilder.DropIndex(
                name: "IX_Companies_AppUserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Companies");
        }
    }
}
