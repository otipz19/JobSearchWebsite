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

namespace Utility.Services.BaseFilterableEntityServices.VacancieService
{
    public class VacancieService : IVacancieService
    {
        private readonly AppDbContext _dbContext;

        public VacancieService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

            string GetShortDescription(string d)
            {
                const int ShortDescriptionLength = 350;
                return $"{d.Substring(0, d.Length < ShortDescriptionLength ? d.Length : ShortDescriptionLength)}...";
            }

            string GetCreatedAgo(DateTime createdAt)
            {
                double fullDays = Math.Floor((DateTime.Now - createdAt).TotalDays);
                createdAt += TimeSpan.FromDays(fullDays);
                double fullHours = Math.Floor((DateTime.Now - createdAt).TotalHours);
                createdAt += TimeSpan.FromHours(fullHours);
                double fullMins = Math.Floor((DateTime.Now - createdAt).TotalMinutes);
                createdAt += TimeSpan.FromMinutes(fullMins);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Created ");
                if (fullDays > 0)
                    stringBuilder.Append($"{fullDays} days ");
                if (fullHours > 0)
                    stringBuilder.Append($"{fullHours} hours ");
                if (fullMins > 0)
                    stringBuilder.Append($"{fullMins} minutes ");
                stringBuilder.Append("ago");
                return stringBuilder.ToString();
            }
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

            async Task<int> GetForeignKey<T>(int sourceId)
                where T : BaseFilteringEntity
            {
                var foreignKey = await _dbContext.Set<T>().Select(e => new { e.Id }).FirstOrDefaultAsync(e => e.Id == sourceId);
                Guard.Against.Null(foreignKey.Id);
                return foreignKey.Id;
            }

            async Task SetCollectionNavProp<T>(List<T> navProp, IEnumerable<string> ids)
                where T : BaseFilteringEntity
            {
                navProp.AddRange(await _dbContext.Set<T>()
                    .Where(e => ids.Select(k => int.Parse(k)).Contains(e.Id))
                    .ToListAsync());
            }
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

        public async Task<List<Vacancie>> EagerLoadVacanciesListAsNoTracking()
        {
            return await IncludeAllNavProps().AsNoTracking().ToListAsync();
        }

        public async Task<Vacancie> EagerLoadVacancieAsNoTracking(int id)
        {
            return await IncludeAllNavProps().AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vacancie> EagerLoadVacancie(int id)
        {
            return await IncludeAllNavProps().FirstOrDefaultAsync(v => v.Id == id);
        }

        public IQueryable<Vacancie> IncludeAllNavProps()
        {
            return _dbContext.Vacancies
                            .Include(v => v.Company)
                            .Include(v => v.States)
                            .Include(v => v.Cities)
                            .Include(v => v.Sphere)
                            .Include(v => v.Specialization)
                            .Include(v => v.Keywords)
                            .Include(v => v.Remoteness)
                            .Include(v => v.ExperienceLevel)
                            .Include(v => v.EnglishLevel);
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

            async Task<List<CheckboxOption>> MapCheckboxOptions(IQueryable<BaseFilteringEntity> filters)
            {
                return await filters.Select(f => new CheckboxOption()
                {
                    IsChecked = false,
                    Text = f.Name,
                    Value = f.Id.ToString(),
                })
                .ToListAsync();
            }
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
