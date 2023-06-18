using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSearchWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVacancieRespond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VacancieRespond",
                columns: table => new
                {
                    VacancieId = table.Column<int>(type: "int", nullable: false),
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancieRespond", x => new { x.ResumeId, x.VacancieId });
                    table.ForeignKey(
                        name: "FK_VacancieRespond_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VacancieRespond_Vacancies_VacancieId",
                        column: x => x.VacancieId,
                        principalTable: "Vacancies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacancieRespond_VacancieId",
                table: "VacancieRespond",
                column: "VacancieId");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME ON \"Resumes\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedResumeCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedResumeCursor\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"ResumeId\";\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedResumeCursor DEALLOCATE DeletedResumeCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE ON \"Vacancies\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedVacancieCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedVacancieCursor\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"VacancieId\";\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedVacancieCursor DEALLOCATE DeletedVacancieCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE;");

            migrationBuilder.DropTable(
                name: "VacancieRespond");
        }
    }
}
