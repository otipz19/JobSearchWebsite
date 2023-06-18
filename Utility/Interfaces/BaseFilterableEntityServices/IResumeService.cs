using Data.Entities;
using System.Security.Claims;
using Utility.ViewModels;

namespace Utility.Interfaces.BaseFilterableEntityServices
{
    public interface IResumeService
    {
        public List<ResumeIndexVm> GetResumeIndexVmList(IEnumerable<Resume> resumes);

        public Task<bool> UserHasAccessTo(ClaimsPrincipal user, Resume resume);

        public Task<Resume> MapViewModelToEntity(ResumeUpsertVm viewModel, Resume resume = null);

        public Task<ResumeUpsertVm> MapEntityToViewModel(Resume resume);

        public Task PopulateVmOnValidationFail(ResumeUpsertVm viewModel);

        public Task<ResumeUpsertVm> GetNewResumeUpsertVm();

        public Task<List<Resume>> EagerLoadListAsNoTracking();

        public Task<Resume> EagerLoadAsNoTracking(int id);

        public Task<Resume> EagerLoad(int id);
    }
}
