using Data.Entities;
using System.Security.Claims;
using Utility.ViewModels;

namespace Utility.Interfaces.BaseFilterableEntityServices
{
    public interface IResumeService
    {
        public List<ResumeIndexVm> GetResumeIndexVmList(IEnumerable<Resume> resumes);

        public bool UserHasAccessTo(ClaimsPrincipal user, Resume resume);

        public Task<Resume> MapViewModelToEntity(ResumeDetailsVm viewModel, Resume resume = null);

        public Task<ResumeDetailsVm> MapEntityToViewModel(Resume resume);

        public Task PopulateVmOnValidationFail(ResumeDetailsVm viewModel);

        public Task<ResumeDetailsVm> GetNewResumeDetailsVm();

        public Task<List<Resume>> EagerLoadListAsNoTracking();

        public Task<Resume> EagerLoadAsNoTracking(int id);

        public Task<Resume> EagerLoad(int id);
    }
}
