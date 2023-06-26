using Ardalis.GuardClauses;
using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Interfaces.Checkbox;
using Utility.Services.Checkbox;
using Utility.ViewModels;

namespace Utility.Services.BaseFilterableEntityServices
{
    public class VacancieService : BaseFilterableEntityService<Vacancie>, IVacancieService
    {
        private readonly ICheckboxService _checkboxService;

		public VacancieService(AppDbContext dbContext,
            ICheckboxService checkboxService) : base(dbContext)
		{
			_checkboxService = checkboxService;
		}

		public List<VacancieIndexVm> GetVacancieIndexVmList(IEnumerable<Vacancie> vacancies)
        {
            return vacancies.Select(v => new VacancieIndexVm()
            {
                Vacancie = v,
                ShortDescription = GetShortDescription(v.Description),
                PublishedAgo = GetPublishedAgo(v.CreatedAt),
            }).ToList();
        }

        public async Task<bool> UserHasAccessTo(ClaimsPrincipal user, Vacancie vacancie)
        {
            if(vacancie.Company == null)
            {
                vacancie.Company = await _dbContext.Companies.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == vacancie.CompanyId);
            }
            return user.FindFirstValue(ClaimTypes.NameIdentifier) == vacancie.Company.AppUserId;
        }

        public async Task<Vacancie> MapViewModelToEntity(VacancieUpsertVm viewModel, Vacancie vacancie = null)
        {
            if (vacancie == null)
            {
                vacancie = new Vacancie();
            }
            else
            {
                vacancie.Keywords.Clear();
                vacancie.States.Clear();
                vacancie.Cities.Clear();
            }

            vacancie.Name = viewModel.Name;
            vacancie.Description = viewModel.Description;
            vacancie.LeftSalaryFork = viewModel.LeftSalaryFork;
            vacancie.RightSalaryFork = viewModel.RightSalaryFork;

            vacancie.SphereId = await GetForeignKey<Sphere>(viewModel.SphereId);
            vacancie.SpecializationId = await GetForeignKey<Specialization>(viewModel.SpecializationId);
            vacancie.RemotenessId = await GetForeignKey<Remoteness>(viewModel.RemotenessId);
            vacancie.ExperienceLevelId = await GetForeignKey<ExperienceLevel>(viewModel.ExperienceLevelId);
            vacancie.EnglishLevelId = await GetForeignKey<EnglishLevel>(viewModel.EnglishLevelId);

            await SetCollectionNavProp(vacancie.Keywords, viewModel.SelectedKeywords);
            await SetCollectionNavProp(vacancie.States, viewModel.SelectedStates);
            await SetCollectionNavProp(vacancie.Cities, viewModel.SelectedCities);

            return vacancie;
        }

        public async Task<VacancieUpsertVm> MapEntityToViewModel(Vacancie vacancie)
        {
            var viewModel = new VacancieUpsertVm()
            {
                Name = vacancie.Name,
                Description = vacancie.Description,
                LeftSalaryFork = vacancie.LeftSalaryFork,
                RightSalaryFork = vacancie.RightSalaryFork,
                SphereId = vacancie.SphereId,
                SpecializationId = vacancie.SpecializationId,
                RemotenessId = vacancie.RemotenessId,
                ExperienceLevelId = vacancie.ExperienceLevelId,
                EnglishLevelId = vacancie.EnglishLevelId,
            };

            await PopulateVM(viewModel);
            SetCheckboxesInVM<BaseFilteringEntity>(viewModel, vacancie.Keywords, vacancie.States, vacancie.Cities, (e) => e.Id);
            return viewModel;
        }

        public async Task PopulateVmOnValidationFail(VacancieUpsertVm viewModel)
        {
            await PopulateVM(viewModel);
            SetCheckboxesInVM(viewModel,
                viewModel.SelectedKeywords,
                viewModel.SelectedStates,
                viewModel.SelectedCities,
                int.Parse);
        }

        public async Task<VacancieUpsertVm> GetNewVacancieUpsertVm()
        {
            VacancieUpsertVm viewModel = new VacancieUpsertVm();
            await PopulateVM(viewModel);
            return viewModel;
        }

        private async Task PopulateVM(VacancieUpsertVm viewModel)
        {
            await AddCheckboxOptionsToVM(viewModel);
            viewModel.AvailableExperienceLevels = await _dbContext.ExperienceLevels.ToListAsync();
            viewModel.AvailableEnglishLevels = await _dbContext.EnglishLevels.ToListAsync();
            viewModel.AvailableRemotenesses = await _dbContext.Remotenesses.ToListAsync();
            viewModel.AvailableSpecializations = await _dbContext.Specializations.ToListAsync();
            viewModel.AvailableSpheres = await _dbContext.Spheres.ToListAsync();
        }

        private async Task AddCheckboxOptionsToVM(VacancieUpsertVm viewModel)
        {
            viewModel.CheckboxKeywords = await _checkboxService.MapFromEntities(_dbContext.Keywords);
            viewModel.CheckboxStates = await _checkboxService.MapFromEntities(_dbContext.States);
            viewModel.CheckboxCities = await _checkboxService.MapFromEntities(_dbContext.Cities);
        }

		private void SetCheckboxesInVM<T>(VacancieUpsertVm viewModel,
			IEnumerable<T> keywords,
			IEnumerable<T> states,
			IEnumerable<T> cities,
			Func<T, int> selector)
		{
			SetCheckboxesInVM(viewModel,
				keywords.Select(selector),
				states.Select(selector),
				cities.Select(selector));
		}

		private void SetCheckboxesInVM(VacancieUpsertVm viewModel,
			IEnumerable<int> keywordsId,
			IEnumerable<int> statesId,
			IEnumerable<int> citiesId)
		{
            _checkboxService.SetIsChecked(viewModel.CheckboxKeywords, keywordsId);
			_checkboxService.SetIsChecked(viewModel.CheckboxStates, statesId);
			_checkboxService.SetIsChecked(viewModel.CheckboxCities, citiesId);
		}
	}
}
