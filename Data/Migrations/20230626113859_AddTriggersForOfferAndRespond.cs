using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSearchWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggersForOfferAndRespond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RespondsCount",
                table: "Vacancies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OffersCount",
                table: "Resumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_JOBOFFER ON \"JobOffers\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldResumeId INT\r\n  DECLARE DeletedJobOfferCursor CURSOR LOCAL FOR SELECT ResumeId FROM Deleted\r\n  OPEN DeletedJobOfferCursor\r\n  FETCH NEXT FROM DeletedJobOfferCursor INTO @OldResumeId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE \"Resumes\"\r\n    SET \"OffersCount\" = \"Resumes\".\"OffersCount\" - 1\r\n    WHERE @OldResumeId = \"Resumes\".\"Id\";\r\n  FETCH NEXT FROM DeletedJobOfferCursor INTO @OldResumeId\r\n  END\r\n  CLOSE DeletedJobOfferCursor DEALLOCATE DeletedJobOfferCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_JOBOFFER ON \"JobOffers\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewResumeId INT\r\n  DECLARE InsertedJobOfferCursor CURSOR LOCAL FOR SELECT ResumeId FROM Inserted\r\n  OPEN InsertedJobOfferCursor\r\n  FETCH NEXT FROM InsertedJobOfferCursor INTO @NewResumeId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE \"Resumes\"\r\n    SET \"OffersCount\" = \"Resumes\".\"OffersCount\" + 1\r\n    WHERE @NewResumeId = \"Resumes\".\"Id\";\r\n  FETCH NEXT FROM InsertedJobOfferCursor INTO @NewResumeId\r\n  END\r\n  CLOSE InsertedJobOfferCursor DEALLOCATE InsertedJobOfferCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIERESPOND ON \"VacancieResponds\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldVacancieId INT\r\n  DECLARE DeletedVacancieRespondCursor CURSOR LOCAL FOR SELECT VacancieId FROM Deleted\r\n  OPEN DeletedVacancieRespondCursor\r\n  FETCH NEXT FROM DeletedVacancieRespondCursor INTO @OldVacancieId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE \"Vacancies\"\r\n    SET \"RespondsCount\" = \"Vacancies\".\"RespondsCount\" - 1\r\n    WHERE @OldVacancieId = \"Vacancies\".\"Id\";\r\n  FETCH NEXT FROM DeletedVacancieRespondCursor INTO @OldVacancieId\r\n  END\r\n  CLOSE DeletedVacancieRespondCursor DEALLOCATE DeletedVacancieRespondCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_VACANCIERESPOND ON \"VacancieResponds\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewVacancieId INT\r\n  DECLARE InsertedVacancieRespondCursor CURSOR LOCAL FOR SELECT VacancieId FROM Inserted\r\n  OPEN InsertedVacancieRespondCursor\r\n  FETCH NEXT FROM InsertedVacancieRespondCursor INTO @NewVacancieId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE \"Vacancies\"\r\n    SET \"RespondsCount\" = \"Vacancies\".\"RespondsCount\" + 1\r\n    WHERE @NewVacancieId = \"Vacancies\".\"Id\";\r\n  FETCH NEXT FROM InsertedVacancieRespondCursor INTO @NewVacancieId\r\n  END\r\n  CLOSE InsertedVacancieRespondCursor DEALLOCATE InsertedVacancieRespondCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_JOBOFFER;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_JOBOFFER;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_VACANCIERESPOND;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_VACANCIERESPOND;");

            migrationBuilder.DropColumn(
                name: "RespondsCount",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "OffersCount",
                table: "Resumes");
        }
    }
}
