using Ardalis.GuardClauses;
using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.ViewModels;

namespace Utility.Services.BaseFilterableEntityServices
{
    public class VacancieService : BaseFilterableEntityService<Vacancie>, IVacancieService
    {
        public VacancieService(AppDbContext dbContext) : base(dbContext)
        {
        }

        public List<VacancieIndexVm> GetVacancieIndexVmList(IEnumerable<Vacancie> vacancies)
        {
            return vacancies.Select(v => new VacancieIndexVm()
            {
                Id = v.Id,
                Name = v.Name,
                ShortDescription = GetShortDescription(v.Description),
                CreatedAgo = GetCreatedAgo(v.CreatedAt),
            }).ToList();
        }

        public bool UserHasAccessTo(ClaimsPrincipal user, Vacancie vacancie)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) == Guard.Against.Null(vacancie.Company).AppUserId;
        }

        public async Task<Vacancie> MapViewModelToEntity(VacancieDetailsVm viewModel, Vacancie vacancie = null)
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

        public async Task<VacancieDetailsVm> MapEntityToViewModel(Vacancie vacancie)
        {
            var viewModel = new VacancieDetailsVm()
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

        public async Task PopulateVmOnValidationFail(VacancieDetailsVm viewModel)
        {
            await PopulateVM(viewModel);
            SetCheckboxesInVM(viewModel,
                viewModel.SelectedKeywords,
                viewModel.SelectedStates,
                viewModel.SelectedCities,
                int.Parse);
        }

        public async Task<VacancieDetailsVm> GetNewVacancieDetailsVm()
        {
            VacancieDetailsVm viewModel = new VacancieDetailsVm();
            await PopulateVM(viewModel);
            return viewModel;
        }

        private async Task PopulateVM(VacancieDetailsVm viewModel)
        {
            await AddCheckboxOptionsToVM(viewModel);
            viewModel.AvailableExperienceLevels = await _dbContext.ExperienceLevels.ToListAsync();
            viewModel.AvailableEnglishLevels = await _dbContext.EnglishLevels.ToListAsync();
            viewModel.AvailableRemotenesses = await _dbContext.Remotenesses.ToListAsync();
            viewModel.AvailableSpecializations = await _dbContext.Specializations.ToListAsync();
            viewModel.AvailableSpheres = await _dbContext.Spheres.ToListAsync();
        }

        private async Task AddCheckboxOptionsToVM(VacancieDetailsVm viewModel)
        {
            viewModel.CheckboxKeywords = await MapCheckboxOptions(_dbContext.Keywords);
            viewModel.CheckboxStates = await MapCheckboxOptions(_dbContext.States);
            viewModel.CheckboxCities = await MapCheckboxOptions(_dbContext.Cities);
        }

        private void SetCheckboxesInVM<T>(VacancieDetailsVm viewModel,
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

        private void SetCheckboxesInVM(VacancieDetailsVm viewModel,
            IEnumerable<int> keywordsId,
            IEnumerable<int> statesId,
            IEnumerable<int> citiesId)
        {
            foreach (int id in keywordsId)
            {
                viewModel.CheckboxKeywords.First(c => c.Id == id).IsChecked = true;
            }
            foreach (int id in statesId)
            {
                viewModel.CheckboxStates.First(c => c.Id == id).IsChecked = true;
            }
            foreach (int id in citiesId)
            {
                viewModel.CheckboxCities.First(c => c.Id == id).IsChecked = true;
            }
        }
    }
}
