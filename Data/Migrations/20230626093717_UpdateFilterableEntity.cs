using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSearchWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFilterableEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountWatched",
                table: "Vacancies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Vacancies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "Vacancies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountWatched",
                table: "Resumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Resumes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "Resumes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountWatched",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "CountWatched",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "Resumes");
        }
    }
}
