using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSearchWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameOfferAndRespond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE;");

            migrationBuilder.DropForeignKey(
                name: "FK_JobOffer_Companies_CompanyId",
                table: "JobOffer");

            migrationBuilder.DropForeignKey(
                name: "FK_JobOffer_Resumes_ResumeId",
                table: "JobOffer");

            migrationBuilder.DropForeignKey(
                name: "FK_JobOffer_Vacancies_VacancieId",
                table: "JobOffer");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancieRespond_Resumes_ResumeId",
                table: "VacancieRespond");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancieRespond_Vacancies_VacancieId",
                table: "VacancieRespond");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VacancieRespond",
                table: "VacancieRespond");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobOffer",
                table: "JobOffer");

            migrationBuilder.RenameTable(
                name: "VacancieRespond",
                newName: "VacancieResponds");

            migrationBuilder.RenameTable(
                name: "JobOffer",
                newName: "JobOffers");

            migrationBuilder.RenameIndex(
                name: "IX_VacancieRespond_VacancieId",
                table: "VacancieResponds",
                newName: "IX_VacancieResponds_VacancieId");

            migrationBuilder.RenameIndex(
                name: "IX_JobOffer_VacancieId",
                table: "JobOffers",
                newName: "IX_JobOffers_VacancieId");

            migrationBuilder.RenameIndex(
                name: "IX_JobOffer_CompanyId",
                table: "JobOffers",
                newName: "IX_JobOffers_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VacancieResponds",
                table: "VacancieResponds",
                columns: new[] { "ResumeId", "VacancieId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobOffers",
                table: "JobOffers",
                columns: new[] { "ResumeId", "CompanyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_JobOffers_Companies_CompanyId",
                table: "JobOffers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobOffers_Resumes_ResumeId",
                table: "JobOffers",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobOffers_Vacancies_VacancieId",
                table: "JobOffers",
                column: "VacancieId",
                principalTable: "Vacancies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VacancieResponds_Resumes_ResumeId",
                table: "VacancieResponds",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VacancieResponds_Vacancies_VacancieId",
                table: "VacancieResponds",
                column: "VacancieId",
                principalTable: "Vacancies",
                principalColumn: "Id");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME ON \"Resumes\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedResumeCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedResumeCursor\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieResponds\"\r\n    WHERE @OldId = \"VacancieResponds\".\"ResumeId\";\r\n    DELETE FROM \"JobOffers\"\r\n    WHERE @OldId = \"JobOffers\".\"ResumeId\";\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedResumeCursor DEALLOCATE DeletedResumeCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE ON \"Vacancies\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedVacancieCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedVacancieCursor\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieResponds\"\r\n    WHERE @OldId = \"VacancieResponds\".\"VacancieId\";\r\n    UPDATE \"JobOffers\"\r\n    SET \"VacancieId\" = NULL\r\n    WHERE @OldId = \"JobOffers\".\"VacancieId\";\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedVacancieCursor DEALLOCATE DeletedVacancieCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE;");

            migrationBuilder.DropForeignKey(
                name: "FK_JobOffers_Companies_CompanyId",
                table: "JobOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_JobOffers_Resumes_ResumeId",
                table: "JobOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_JobOffers_Vacancies_VacancieId",
                table: "JobOffers");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancieResponds_Resumes_ResumeId",
                table: "VacancieResponds");

            migrationBuilder.DropForeignKey(
                name: "FK_VacancieResponds_Vacancies_VacancieId",
                table: "VacancieResponds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VacancieResponds",
                table: "VacancieResponds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobOffers",
                table: "JobOffers");

            migrationBuilder.RenameTable(
                name: "VacancieResponds",
                newName: "VacancieRespond");

            migrationBuilder.RenameTable(
                name: "JobOffers",
                newName: "JobOffer");

            migrationBuilder.RenameIndex(
                name: "IX_VacancieResponds_VacancieId",
                table: "VacancieRespond",
                newName: "IX_VacancieRespond_VacancieId");

            migrationBuilder.RenameIndex(
                name: "IX_JobOffers_VacancieId",
                table: "JobOffer",
                newName: "IX_JobOffer_VacancieId");

            migrationBuilder.RenameIndex(
                name: "IX_JobOffers_CompanyId",
                table: "JobOffer",
                newName: "IX_JobOffer_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VacancieRespond",
                table: "VacancieRespond",
                columns: new[] { "ResumeId", "VacancieId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobOffer",
                table: "JobOffer",
                columns: new[] { "ResumeId", "CompanyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_JobOffer_Companies_CompanyId",
                table: "JobOffer",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobOffer_Resumes_ResumeId",
                table: "JobOffer",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobOffer_Vacancies_VacancieId",
                table: "JobOffer",
                column: "VacancieId",
                principalTable: "Vacancies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VacancieRespond_Resumes_ResumeId",
                table: "VacancieRespond",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VacancieRespond_Vacancies_VacancieId",
                table: "VacancieRespond",
                column: "VacancieId",
                principalTable: "Vacancies",
                principalColumn: "Id");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_RESUME ON \"Resumes\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedResumeCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedResumeCursor\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"ResumeId\";\r\n    DELETE FROM \"JobOffer\"\r\n    WHERE @OldId = \"JobOffer\".\"ResumeId\";\r\n  FETCH NEXT FROM DeletedResumeCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedResumeCursor DEALLOCATE DeletedResumeCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIE ON \"Vacancies\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldId INT\r\n  DECLARE DeletedVacancieCursor CURSOR LOCAL FOR SELECT Id FROM Deleted\r\n  OPEN DeletedVacancieCursor\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    DELETE FROM \"VacancieRespond\"\r\n    WHERE @OldId = \"VacancieRespond\".\"VacancieId\";\r\n    UPDATE \"JobOffer\"\r\n    SET \"VacancieId\" = NULL\r\n    WHERE @OldId = \"JobOffer\".\"VacancieId\";\r\n  FETCH NEXT FROM DeletedVacancieCursor INTO @OldId\r\n  END\r\n  CLOSE DeletedVacancieCursor DEALLOCATE DeletedVacancieCursor\r\nEND");
        }
    }
}
