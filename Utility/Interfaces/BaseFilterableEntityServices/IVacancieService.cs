using Data.Entities;
using System.Security.Claims;
using Utility.ViewModels;

namespace Utility.Interfaces.BaseFilterableEntityServices
{
    public interface IVacancieService
    {
        public List<VacancieIndexVm> GetVacancieIndexVmList(IEnumerable<Vacancie> vacancies);

        public Task<bool> UserHasAccessTo(ClaimsPrincipal user, Vacancie vacancie);

        public Task<Vacancie> MapViewModelToEntity(VacancieUpsertVm viewModel, Vacancie vacancie = null);

        public Task<VacancieUpsertVm> MapEntityToViewModel(Vacancie vacancie);

        public Task PopulateVmOnValidationFail(VacancieUpsertVm viewModel);

        public Task<VacancieUpsertVm> GetNewVacancieUpsertVm();

        public Task<List<Vacancie>> EagerLoadListAsNoTracking();

        public Task<Vacancie> EagerLoadAsNoTracking(int id);

        public Task<Vacancie> EagerLoad(int id);
    }
}
