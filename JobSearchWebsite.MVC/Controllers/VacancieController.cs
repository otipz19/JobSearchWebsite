using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using FluentValidation.AspNetCore;
using Utility.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Toaster;
using Utility.Utilities;
using Utility.Interfaces.BaseFilterableEntityServices;

namespace JobSearchWebsite.MVC.Controllers
{
    public class VacancieController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidator<VacancieDetailsVm> _validator;
        private readonly IVacancieService _vacancieService;

        public VacancieController(AppDbContext dbContext, IValidator<VacancieDetailsVm> validator, IVacancieService vacancieService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _vacancieService = vacancieService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vacancies = _vacancieService.GetVacancieIndexVmList(await _dbContext.Vacancies.ToListAsync());
            return View(vacancies);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Vacancie vacancie = await _vacancieService.EagerLoadVacancieAsNoTracking(id);
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
            var viewModel = await _vacancieService.GetNewVacancieDetailsVm();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Create(VacancieDetailsVm viewModel)
        {
            var validationResult = await _validator.ValidateAsync(viewModel);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                await _vacancieService.PopulateVmOnValidationFail(viewModel);
                return View(viewModel);
            }

            Vacancie vacancie;
            try
            {
                vacancie = await _vacancieService.MapViewModelToEntity(viewModel);
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
            Vacancie toUpdate = await _vacancieService.EagerLoadVacancieAsNoTracking(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            if (!_vacancieService.UserHasAccessTo(User, toUpdate))
            {
                return Forbid();
            }
            return View(await _vacancieService.MapEntityToViewModel(toUpdate));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Update(int id, VacancieDetailsVm viewModel)
        {
            var validationResult = await _validator.ValidateAsync(viewModel);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                await _vacancieService.PopulateVmOnValidationFail(viewModel);
                return View(viewModel);
            }

            Vacancie toUpdate = await _vacancieService.EagerLoadVacancie(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            if (!_vacancieService.UserHasAccessTo(User, toUpdate))
            {
                return Forbid();
            }

            try
            {
                await _vacancieService.MapViewModelToEntity(viewModel, toUpdate);
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
            if (!_vacancieService.UserHasAccessTo(User, toDelete))
            {
                return Forbid();
            }

            _dbContext.Vacancies.Remove(toDelete);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Vacancie was deleted successfully");
            return RedirectToAction(nameof(Index));
        }
    }
}
