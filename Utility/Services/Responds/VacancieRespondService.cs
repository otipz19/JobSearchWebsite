using Ardalis.GuardClauses;
using Azure;
using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Exceptions;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Interfaces.Responds;

namespace Utility.Services.Responds
{
	public class VacancieRespondService : IVacancieRespondService
	{
		private readonly AppDbContext _dbContext;
		private readonly IVacancieService _vacancieService;

		public VacancieRespondService(AppDbContext dbContext, IVacancieService vacancieService)
		{
			_dbContext = dbContext;
			_vacancieService = vacancieService;
		}

		public async Task CreateVacancieRespond(int resumeId, int vacancieId)
		{
			VacancieRespond vacancieRespond = await _dbContext.VacancieResponds.AsNoTracking()
				.FirstOrDefaultAsync(r => r.ResumeId == resumeId && r.VacancieId == vacancieId);
			//If connection already exist, i.e. vacancie has been already responded by this resume
			if (vacancieRespond != null)
			{
				throw new ApplicationException("VacancieRespond already exists");
			}

			vacancieRespond = new VacancieRespond()
			{
				ResumeId = resumeId,
				VacancieId = vacancieId,
			};
			_dbContext.VacancieResponds.Add(vacancieRespond);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<List<VacancieRespond>> GetVacancieRespondsForJobseeker(ClaimsPrincipal user)
		{
			Jobseeker jobseeker = await _dbContext.Jobseekers.AsNoTracking()
					.Include(j => j.Resumes)
						.ThenInclude(r => r.VacancieResponds)
					.FirstOrDefaultAsync(j => j.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
			Guard.Against.Null(jobseeker);
			return jobseeker.Resumes.SelectMany(r => r.VacancieResponds).ToList();
		}

		public async Task<List<VacancieRespond>> GetVacancieRespondsForCompany(ClaimsPrincipal user)
		{
			Company company = await _dbContext.Companies.AsNoTracking()
					.Include(c => c.Vacancies)
						.ThenInclude(v => v.VacancieResponds)
					.FirstOrDefaultAsync(c => c.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
			Guard.Against.Null(company);
			return company.Vacancies.SelectMany(v => v.VacancieResponds).ToList();
		}

		public async Task<VacancieRespond> GetVacancieRespondForCompany(ClaimsPrincipal user, int resumeId, int vacancieId)
		{
			Vacancie vacancie = await _dbContext.Vacancies
				.Include(v => v.VacancieResponds
					.Where(respond => respond.VacancieId == v.Id && respond.ResumeId == resumeId))
				.FirstOrDefaultAsync(v => v.Id == vacancieId);

			Guard.Against.Null(vacancie);
			if(! await _vacancieService.UserHasAccessTo(user, vacancie))
			{
				throw new NoAccessException();
			}
			return Guard.Against.Null(vacancie.VacancieResponds.FirstOrDefault());
		}

		public async Task ChangeStatus(VacancieRespond respond, VacancieRespondStatus status)
		{
			if (respond.Status == VacancieRespondStatus.Accepted || respond.Status == VacancieRespondStatus.Rejected)
			{
				throw new ArgumentException();
			}

			respond.Status = status;
			_dbContext.VacancieResponds.Update(respond);
			await _dbContext.SaveChangesAsync();
		}
	}
}
