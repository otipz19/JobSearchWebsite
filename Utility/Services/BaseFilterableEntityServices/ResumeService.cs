using Ardalis.GuardClauses;
using Data;
using Data.Entities;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.ViewModels;

namespace Utility.Services.BaseFilterableEntityServices
{
    public class ResumeService : BaseFilterableEntityService<Resume>, IResumeService
    {
        public ResumeService(AppDbContext dbContext) : base(dbContext)
        {
        }

        public List<ResumeIndexVm> GetResumeIndexVmList(IEnumerable<Resume> resumes)
        {
            return resumes.Select(r => new ResumeIndexVm()
            {
                Id = r.Id,
                Name = r.Name,
                CreatedAgo = GetCreatedAgo(r.CreatedAt),
                ShortDescription = GetShortDescription(r.Description),
            }).ToList();
        }

        public async Task<ResumeDetailsVm> GetNewResumeDetailsVm()
        {
            ResumeDetailsVm viewModel = new ResumeDetailsVm();
            await PopulateVM(viewModel);
            return viewModel;
        }

        public async Task<ResumeDetailsVm> MapEntityToViewModel(Resume resume)
        {
            ResumeDetailsVm viewModel = new ResumeDetailsVm()
            {
                Name = resume.Name,
                Description = resume.Description,
                WantedSalary = resume.WantedSalary,
                SphereId = resume.SphereId,
                SpecializationId = resume.SpecializationId,
                RemotenessId = resume.RemotenessId,
                ExperienceLevelId = resume.ExperienceLevelId,
                EnglishLevelId = resume.EnglishLevelId,
                StateId = resume.StateId,
                CityId = resume.CityId,
            };

            await PopulateVM(viewModel);
            SetCheckboxesInVM(viewModel, resume.Keywords, e => e.Id);
            return viewModel;
        }

        public async Task<Resume> MapViewModelToEntity(ResumeDetailsVm viewModel, Resume resume = null)
        {
            if (resume == null)
                resume = new Resume();
            else
                resume.Keywords.Clear();

            resume.Name = viewModel.Name;
            resume.Description = viewModel.Description;
            resume.WantedSalary = viewModel.WantedSalary;

            resume.SphereId = await GetForeignKey<Sphere>(viewModel.SphereId);
            resume.SpecializationId = await GetForeignKey<Specialization>(viewModel.SpecializationId);
            resume.RemotenessId = await GetForeignKey<Remoteness>(viewModel.RemotenessId);
            resume.ExperienceLevelId = await GetForeignKey<ExperienceLevel>(viewModel.ExperienceLevelId);
            resume.EnglishLevelId = await GetForeignKey<EnglishLevel>(viewModel.EnglishLevelId);
            resume.StateId = await GetForeignKey<State>(viewModel.StateId);
            resume.CityId = await GetForeignKey<City>(viewModel.CityId);

            await SetCollectionNavProp(resume.Keywords, viewModel.SelectedKeywords);

            return resume;
        }

        public async Task PopulateVmOnValidationFail(ResumeDetailsVm viewModel)
        {
            await PopulateVM(viewModel);
            SetCheckboxesInVM(viewModel,
                viewModel.SelectedKeywords,
                int.Parse);
        }

        public bool UserHasAccessTo(ClaimsPrincipal user, Resume resume)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) == Guard.Against.Null(resume.Jobseeker).AppUserId;
        }

        private async Task PopulateVM(ResumeDetailsVm viewModel)
        {
            await AddCheckboxOptionsToVM(viewModel);
            viewModel.AvailableExperienceLevels = await _dbContext.ExperienceLevels.ToListAsync();
            viewModel.AvailableEnglishLevels = await _dbContext.EnglishLevels.ToListAsync();
            viewModel.AvailableRemotenesses = await _dbContext.Remotenesses.ToListAsync();
            viewModel.AvailableSpecializations = await _dbContext.Specializations.ToListAsync();
            viewModel.AvailableSpheres = await _dbContext.Spheres.ToListAsync();
            viewModel.AvailableCities = await _dbContext.Cities.ToListAsync();
            viewModel.AvailableStates = await _dbContext.States.ToListAsync();
        }

        private async Task AddCheckboxOptionsToVM(ResumeDetailsVm viewModel)
        {
            viewModel.CheckboxKeywords = await MapCheckboxOptions(_dbContext.Keywords);
        }

        private void SetCheckboxesInVM<T>(ResumeDetailsVm viewModel,
            IEnumerable<T> keywords,
            Func<T, int> selector)
        {
            SetCheckboxesInVM(viewModel,
                keywords.Select(selector));
        }

        private void SetCheckboxesInVM(ResumeDetailsVm viewModel,
            IEnumerable<int> keywordsId)
        {
            foreach (int id in keywordsId)
            {
                viewModel.CheckboxKeywords.First(c => c.Id == id).IsChecked = true;
            }
        }
    }
}
