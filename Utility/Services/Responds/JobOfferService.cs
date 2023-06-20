using Ardalis.GuardClauses;
using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Exceptions;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Interfaces.Responds;
using Utility.Utilities;
using Utility.ViewModels;

namespace Utility.Services.Responds
{
    public class JobOfferService : IJobOfferService
    {
        private readonly AppDbContext _dbContext;
        private readonly IResumeService _resumeService;

        public JobOfferService(AppDbContext dbContext,
            IResumeService resumeService)
        {
            _dbContext = dbContext;
            _resumeService = resumeService;
        }

        public async Task<List<JobOffer>> GetJobOffersForJobseeker(ClaimsPrincipal user)
        {
            Jobseeker jobseeker = await _dbContext.Jobseekers.AsNoTracking()
                    .Include(j => j.Resumes)
                        .ThenInclude(r => r.JobOffers)
                            .ThenInclude(o => o.Vacancie)
                    .FirstOrDefaultAsync(j => j.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
            Guard.Against.Null(jobseeker);
            return jobseeker.Resumes.SelectMany(r => r.JobOffers).ToList();
        }

        public async Task<List<JobOffer>> GetJobOffersForCompany(ClaimsPrincipal user)
        {
            Company company = await _dbContext.Companies.AsNoTracking()
                   .Include(c => c.Vacancies)
                       .ThenInclude(v => v.JobOffers)
                           .ThenInclude(o => o.Resume)
                   .FirstOrDefaultAsync(c => c.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
            Guard.Against.Null(company);
            return company.Vacancies.SelectMany(v => v.JobOffers).ToList();
        }

        public List<JobOfferIndexVm> GetIndexVmList(IEnumerable<JobOffer> offers)
        {
            return offers.Select(GetIndexVm).ToList();
        }

        public JobOfferIndexVm GetIndexVm(JobOffer offer)
        {
            return new JobOfferIndexVm()
            {
                JobOffer = offer,
                SentAgo = GetSentAgo(),
                AnsweredAgo = GetAnsweredAgo(),
            };

            string GetSentAgo()
            {
                var str = offer.CreatedAt.GetTimePassedString();
                return str == "" ? "Just sent" : "Sent" + str;
            }

            string GetAnsweredAgo()
            {
                if (offer.StatusChangedAt is null)
                    return "";
                var str = offer.StatusChangedAt.GetTimePassedString();
                return str == "" ? "Just answered" : "Answered" + str;
            }
        }

        public async Task<JobOffer> GetJobOfferForJobseeker(ClaimsPrincipal user, int resumeId, int companyId)
        {
            Resume resume = await _dbContext.Resumes.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == resumeId);

            Guard.Against.Null(resume);

            resume.JobOffers = await _dbContext.JobOffers
                .Where(o => o.ResumeId == resume.Id && o.CompanyId == companyId)
                .ToListAsync();

            if (!await _resumeService.UserHasAccessTo(user, resume))
            {
                throw new NoAccessException();
            }
            return Guard.Against.Null(resume.JobOffers.FirstOrDefault());
        }

        public async Task ChangeStatus(JobOffer offer, RespondStatus status)
        {
            if (offer.Status == RespondStatus.Accepted || offer.Status == RespondStatus.Rejected)
            {
                throw new ArgumentException();
            }

            offer.Status = status;
            offer.StatusChangedAt = DateTime.Now;
            _dbContext.JobOffers.Update(offer);
            await _dbContext.SaveChangesAsync();
        }
    }
}
