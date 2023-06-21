using Ardalis.GuardClauses;
using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Exceptions;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Interfaces.Responds;
using Utility.ViewModels;
using Utility.Utilities;
using Microsoft.IdentityModel.Tokens;

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
							.ThenInclude(respond => respond.Vacancie)
					.FirstOrDefaultAsync(j => j.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
			Guard.Against.Null(jobseeker);
			return jobseeker.Resumes.SelectMany(r => r.VacancieResponds).ToList();
		}

		public async Task<List<VacancieRespond>> GetVacancieRespondsForCompany(ClaimsPrincipal user)
		{
			Company company = await _dbContext.Companies.AsNoTracking()
					.Include(c => c.Vacancies)
						.ThenInclude(v => v.VacancieResponds)
							.ThenInclude(respond => respond.Resume)
					.FirstOrDefaultAsync(c => c.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
			Guard.Against.Null(company);
			return company.Vacancies.SelectMany(v => v.VacancieResponds).ToList();
		}

		public async Task<VacancieRespond> GetVacancieRespondForCompany(ClaimsPrincipal user, int resumeId, int vacancieId)
		{
			Vacancie vacancie = await _dbContext.Vacancies.AsNoTracking()
				.FirstOrDefaultAsync(v => v.Id == vacancieId);

            Guard.Against.Null(vacancie);

            vacancie.VacancieResponds = await _dbContext.VacancieResponds
				.Where(respond => respond.VacancieId == vacancie.Id && respond.ResumeId == resumeId)
				.ToListAsync();

			if(! await _vacancieService.UserHasAccessTo(user, vacancie))
			{
				throw new NoAccessException();
			}
			return Guard.Against.Null(vacancie.VacancieResponds.FirstOrDefault());
		}

		public async Task ChangeStatus(VacancieRespond respond, RespondStatus status)
		{
			if (respond.Status == RespondStatus.Accepted || respond.Status == RespondStatus.Rejected)
			{
				throw new ArgumentException();
			}

			respond.Status = status;
			respond.StatusChangedAt = DateTime.Now;
			_dbContext.VacancieResponds.Update(respond);
			await _dbContext.SaveChangesAsync();
		}

		public VacancieRespondIndexVm GetIndexVm(IEnumerable<VacancieRespond> responds, Vacancie vacancie = null, Resume resume = null)
		{
			return new VacancieRespondIndexVm()
			{
				VacancieRepsonds = responds.Select(GetDetailsVm).ToList(),
				CommonVacancie = vacancie,
				CommonResume = resume,
			};
		}

		public VacancieRespondDetailsVm GetDetailsVm(VacancieRespond respond)
		{
			return new VacancieRespondDetailsVm()
			{
				VacancieRespond = respond,
				SentAgo = GetSentAgo(),
				AnsweredAgo = GetAnsweredAgo(), 
			};

			string GetSentAgo()
			{
				var str = respond.CreatedAt.GetTimePassedString();
                return str == "" ? "Just sent" : "Sent" + str;
            }

			string GetAnsweredAgo()
			{
				if (respond.StatusChangedAt is null)
					return "";
				var str = respond.StatusChangedAt.GetTimePassedString();
				return str == "" ? "Just answered" : "Answered" + str;
            }
        }
	}
}
