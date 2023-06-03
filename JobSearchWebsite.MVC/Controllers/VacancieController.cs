using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using FluentValidation.AspNetCore;
using JobSearchWebsite.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Toaster;
using Utility.Utilities;

namespace JobSearchWebsite.MVC.Controllers
{
    public class VacancieController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidator<VacancieVM> _validator;

        public VacancieController(AppDbContext dbContext, IValidator<VacancieVM> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vacancies = await EagerLoadVacanciesListAsNoTracking();
            return View(vacancies);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Vacancie vacancie = await EagerLoadVacancieAsNoTracking(id);
            if (vacancie == null)
            {
                return NotFound();
            }
            return View(vacancie);
        }

        [HttpGet]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Create()
        {
            var viewModel = await PopulateVM(new VacancieVM());
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Create(VacancieVM viewModel)
        {
            var validationResult = await _validator.ValidateAsync(viewModel);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                await PopulateVM(viewModel);
                SetCheckboxesInVM(viewModel,
                    viewModel.SelectedKeywords,
                    viewModel.SelectedStates,
                    viewModel.SelectedCities,
                    (e) => int.Parse(e));
                return View(viewModel);
            }

            Vacancie vacancie;
            try
            {
                vacancie = await MapViewModelToEntity(viewModel);
            }
            catch
            {
                return NotFound();
            }

            Company company = await _dbContext.Companies
                .FirstOrDefaultAsync(c => c.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (company == null)
            {
                return NotFound();
            }
            vacancie.Company = company;

            _dbContext.Vacancies.Add(vacancie);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Vacancie was created succesfully");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Update(int id)
        {
            Vacancie toUpdate = await EagerLoadVacancieAsNoTracking(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            if (!UserHasAccessTo(toUpdate))
            {
                return Forbid();
            }
            return View(await MapEntityToViewModel(toUpdate));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Update(int id, VacancieVM viewModel)
        {
            var validationResult = await _validator.ValidateAsync(viewModel);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                await PopulateVM(viewModel);
                SetCheckboxesInVM(viewModel,
                    viewModel.SelectedKeywords,
                    viewModel.SelectedStates,
                    viewModel.SelectedCities,
                    (e) => int.Parse(e));
                return View(viewModel);
            }

            Vacancie toUpdate = await EagerLoadVacancie(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            if (!UserHasAccessTo(toUpdate))
            {
                return Forbid();
            }

            try
            {
                await MapViewModelToEntity(viewModel, toUpdate);
            }
            catch
            {
                return NotFound();
            }

            _dbContext.Vacancies.Update(toUpdate);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Vacancie was edited succesfully");
            return RedirectToAction(nameof(Details), new { id = toUpdate.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Delete(int id)
        {
            Vacancie toDelete = await _dbContext.Vacancies
                .Include(v => v.Company)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (toDelete == null)
            {
                return NotFound();
            }
            if (!UserHasAccessTo(toDelete))
            {
                return Forbid();
            }

            _dbContext.Vacancies.Remove(toDelete);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Vacancie was deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool UserHasAccessTo(Vacancie vacancie) =>
            vacancie.Company.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task<VacancieVM> MapEntityToViewModel(Vacancie vacancie)
        {
            var viewModel = new VacancieVM()
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

        private void SetCheckboxesInVM<T>(VacancieVM viewModel,
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

        private void SetCheckboxesInVM(VacancieVM viewModel,
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

        private async Task<Vacancie> MapViewModelToEntity(VacancieVM viewModel, Vacancie vacancie = null)
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
                if (foreignKey == null)
                    throw new ApplicationException();
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

        private async Task<VacancieVM> PopulateVM(VacancieVM viewModel)
        {
            await AddCheckboxOptionsToVM(viewModel);
            viewModel.AvailableExperienceLevels = await _dbContext.ExperienceLevels.ToListAsync();
            viewModel.AvailableEnglishLevels = await _dbContext.EnglishLevels.ToListAsync();
            viewModel.AvailableRemotenesses = await _dbContext.Remotenesses.ToListAsync();
            viewModel.AvailableSpecializations = await _dbContext.Specializations.ToListAsync();
            viewModel.AvailableSpheres = await _dbContext.Spheres.ToListAsync();
            return viewModel;
        }

        private async Task<VacancieVM> AddCheckboxOptionsToVM(VacancieVM viewModel)
        {
            viewModel.CheckboxKeywords = await MapCheckboxOptions(_dbContext.Keywords);
            viewModel.CheckboxStates = await MapCheckboxOptions(_dbContext.States);
            viewModel.CheckboxCities = await MapCheckboxOptions(_dbContext.Cities);
            return viewModel;

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

        private async Task<List<Vacancie>> EagerLoadVacanciesListAsNoTracking()
        {
            return await IncludeAllNavProps().AsNoTracking().ToListAsync();
        }

        private async Task<Vacancie> EagerLoadVacancieAsNoTracking(int id)
        {
            return await IncludeAllNavProps().AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        }

        private async Task<Vacancie> EagerLoadVacancie(int id)
        {
            return await IncludeAllNavProps().FirstOrDefaultAsync(v => v.Id == id);
        }

        private IQueryable<Vacancie> IncludeAllNavProps()
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
    }
}
