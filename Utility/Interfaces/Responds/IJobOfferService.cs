using Ardalis.GuardClauses;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Exceptions;
using Utility.Utilities;
using Utility.ViewModels;

namespace Utility.Interfaces.Responds
{
    public interface IJobOfferService
    {
        public Task CreateJobOffer(int resumeId, int companyId, int? vacancieId = null, string message = null);

        public Task<List<JobOffer>> GetJobOffersForJobseeker(ClaimsPrincipal user);

        public Task<List<JobOffer>> GetJobOffersForCompany(ClaimsPrincipal user);

        public JobOfferIndexVm GetIndexVm(IEnumerable<JobOffer> offers, Vacancie vacancie = null, Resume resume = null);

        public JobOfferDetailsVm GetDetailsVm(JobOffer offer);

        public Task<JobOffer> GetJobOfferForJobseeker(ClaimsPrincipal user, int resumeId, int companyId);

        public Task ChangeStatus(JobOffer offer, RespondStatus status);
    }
}
