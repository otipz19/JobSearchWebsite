using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSearchWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobseekerAndResume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Jobseekers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    About = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobseekers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WantedSalary = table.Column<int>(type: "int", nullable: false),
                    JobseekerId = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SphereId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false),
                    RemotenessId = table.Column<int>(type: "int", nullable: false),
                    ExperienceLevelId = table.Column<int>(type: "int", nullable: false),
                    EnglishLevelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resumes_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_EnglishLevels_EnglishLevelId",
                        column: x => x.EnglishLevelId,
                        principalTable: "EnglishLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_ExperienceLevels_ExperienceLevelId",
                        column: x => x.ExperienceLevelId,
                        principalTable: "ExperienceLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_Jobseekers_JobseekerId",
                        column: x => x.JobseekerId,
                        principalTable: "Jobseekers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_Remotenesses_RemotenessId",
                        column: x => x.RemotenessId,
                        principalTable: "Remotenesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_Spheres_SphereId",
                        column: x => x.SphereId,
                        principalTable: "Spheres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resumes_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeywordResume",
                columns: table => new
                {
                    KeywordsId = table.Column<int>(type: "int", nullable: false),
                    ResumesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordResume", x => new { x.KeywordsId, x.ResumesId });
                    table.ForeignKey(
                        name: "FK_KeywordResume_Keywords_KeywordsId",
                        column: x => x.KeywordsId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeywordResume_Resumes_ResumesId",
                        column: x => x.ResumesId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeywordResume_ResumesId",
                table: "KeywordResume",
                column: "ResumesId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_CityId",
                table: "Resumes",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_EnglishLevelId",
                table: "Resumes",
                column: "EnglishLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_ExperienceLevelId",
                table: "Resumes",
                column: "ExperienceLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_JobseekerId",
                table: "Resumes",
                column: "JobseekerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_RemotenessId",
                table: "Resumes",
                column: "RemotenessId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_SpecializationId",
                table: "Resumes",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_SphereId",
                table: "Resumes",
                column: "SphereId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_StateId",
                table: "Resumes",
                column: "StateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeywordResume");

            migrationBuilder.DropTable(
                name: "Resumes");

            migrationBuilder.DropTable(
                name: "Jobseekers");

            migrationBuilder.DropColumn(
                name: "About",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Companies");
        }
    }
}
