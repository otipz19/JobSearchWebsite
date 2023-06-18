using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSearchWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE;");

            migrationBuilder.CreateTable(
                name: "JobOffer",
                columns: table => new
                {
                    ResumeId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    VacancieId = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOffer", x => new { x.ResumeId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_JobOffer_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobOffer_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobOffer_Vacancies_VacancieId",
                        column: x => x.VacancieId,
                        principalTable: "Vacancies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_CompanyId",
                table: "JobOffer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOffer_VacancieId",
                table: "JobOffer",
                column: "VacancieId");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME ON \"Resumes\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedResumeCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedResumeCursor\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"ResumeId\";\r\n    DELETE FROM \"JobOffer\"\r\n    WHERE @OldId = \"JobOffer\".\"ResumeId\";\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedResumeCursor DEALLOCATE DeletedResumeCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE ON \"Vacancies\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedVacancieCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedVacancieCursor\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"VacancieId\";\r\n    UPDATE \"JobOffer\"\r\n    SET \"VacancieId\" = NULL\r\n    WHERE @OldId = \"JobOffer\".\"VacancieId\";\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedVacancieCursor DEALLOCATE DeletedVacancieCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE;");

            migrationBuilder.DropTable(
                name: "JobOffer");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME ON \"Resumes\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedResumeCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedResumeCursor\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"ResumeId\";\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedResumeCursor DEALLOCATE DeletedResumeCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE ON \"Vacancies\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedVacancieCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedVacancieCursor\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"VacancieId\";\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedVacancieCursor DEALLOCATE DeletedVacancieCursor\r\nEND");
        }
    }
}
