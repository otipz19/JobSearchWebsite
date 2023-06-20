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
        public Task<List<JobOffer>> GetJobOffersForJobseeker(ClaimsPrincipal user);

        public Task<List<JobOffer>> GetJobOffersForCompany(ClaimsPrincipal user);

        public List<JobOfferIndexVm> GetIndexVmList(IEnumerable<JobOffer> offers);

        public JobOfferIndexVm GetIndexVm(JobOffer offer);

        public Task<JobOffer> GetJobOfferForJobseeker(ClaimsPrincipal user, int resumeId, int companyId);

        public Task ChangeStatus(JobOffer offer, RespondStatus status);
    }
}
