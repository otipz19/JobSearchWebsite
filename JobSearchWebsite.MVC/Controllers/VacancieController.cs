using Data;
using Data.Entities;
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
        private readonly IValidator<Vacancie> _validator;

        public VacancieController(AppDbContext dbContext, IValidator<Vacancie> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Vacancie> vacancies = await _dbContext.Vacancies.AsNoTracking().ToListAsync();
            return View(vacancies);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Vacancie vacancie = await _dbContext.Vacancies.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if(vacancie == null)
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
            await PopulateVM(viewModel);

            var validationResult = await _validator.ValidateAsync(viewModel.Vacancie);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                return View(viewModel);
            }

            //TODO: Create logic should be advanced
            _dbContext.Vacancies.Add(viewModel.Vacancie);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Vacancie was created succesfully");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Update(int id)
        {
            Vacancie toUpdate = await EagerLoadVacancie(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            if (!UserHasAccessTo(toUpdate))
            {
                return Forbid();
            }
            return View(await PopulateVM(new VacancieVM()
            {
                Vacancie = toUpdate,
            }));
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Update(VacancieVM viewModel)
        {
            await PopulateVM(viewModel);

            var validationResult = await _validator.ValidateAsync(viewModel.Vacancie);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                return View(viewModel);
            }

            Vacancie toUpdate = await EagerLoadVacancie(viewModel.Vacancie.Id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            if (!UserHasAccessTo(toUpdate))
            {
                return Forbid();
            }

            //TODO: Update logic should be advanced
            toUpdate.Update(viewModel.Vacancie);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Vacancie was edited succesfully");
            return RedirectToAction(nameof(Details), new {id = toUpdate.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Vacancie toDelete = await EagerLoadVacancie(id);
            if (toDelete == null)
            {
                return NotFound();
            }
            if(!UserHasAccessTo(toDelete))
            {
                return Forbid();
            }
            return View(await PopulateVM(new VacancieVM()
            {
                Vacancie = toDelete,
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            Vacancie toDelete = await _dbContext.Vacancies
                .Include(v => v.Company)
                .FirstOrDefaultAsync(v => v.Id == id);
            if(toDelete == null)
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
            vacancie.Company.AppUserId != User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task<VacancieVM> PopulateVM(VacancieVM viewModel)
        {
            viewModel.AvailableCitites = await _dbContext.Cities.ToListAsync();
            viewModel.AvailableStates = await _dbContext.States.ToListAsync();
            viewModel.AvailableExperienceLevels = await _dbContext.ExperienceLevels.ToListAsync();
            viewModel.AvailableEnglishLevels = await _dbContext.EnglishLevels.ToListAsync();
            viewModel.AvailableRemotenesses = await _dbContext.Remotenesses.ToListAsync();
            viewModel.AvailableKeywords = await _dbContext.Keywords.ToListAsync();
            viewModel.AvailableSpecializations = await _dbContext.Specializations.ToListAsync();
            viewModel.AvailableSpheres = await _dbContext.Spheres.ToListAsync();
            return viewModel;
        }

        private async Task<Vacancie> EagerLoadVacancie(int id)
        {
            return await _dbContext.Vacancies
                .Include(v => v.Company)
                .Include(v => v.States)
                .Include(v => v.Cities)
                .Include(v => v.Sphere)
                .Include(v => v.Specialization)
                .Include(v => v.Keywords)
                .Include(v => v.Remoteness)
                .Include(v => v.ExperienceLevel)
                .Include(v => v.EnglishLevel)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}
