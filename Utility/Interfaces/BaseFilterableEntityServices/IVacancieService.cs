using Data.Entities;
using System.Security.Claims;
using Utility.ViewModels;

namespace Utility.Interfaces.BaseFilterableEntityServices
{
    public interface IVacancieService
    {
        public List<VacancieIndexVm> GetVacancieIndexVmList(IEnumerable<Vacancie> vacancies);

        public bool UserHasAccessTo(ClaimsPrincipal user, Vacancie vacancie);

        public Task<Vacancie> MapViewModelToEntity(VacancieDetailsVm viewModel, Vacancie vacancie = null);

        public Task<VacancieDetailsVm> MapEntityToViewModel(Vacancie vacancie);

        public Task PopulateVmOnValidationFail(VacancieDetailsVm viewModel);

        public Task<VacancieDetailsVm> GetNewVacancieDetailsVm();

        public Task<List<Vacancie>> EagerLoadListAsNoTracking();

        public Task<Vacancie> EagerLoadAsNoTracking(int id);

        public Task<Vacancie> EagerLoad(int id);
    }
}
