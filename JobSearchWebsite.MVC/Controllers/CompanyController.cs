using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.Utilities;
using Utility.Interfaces.FileUpload.Image;
using Microsoft.EntityFrameworkCore;
using Utility.ViewModels;
using Utility.Interfaces.BaseFilterableEntityServices;
using System.Security.Claims;

namespace JobSearchWebsite.MVC.Controllers
{
    public class CompanyController : BaseProfileEntityController<Company>
    {
        private readonly IVacancieService _vacancieService;

        public CompanyController(AppDbContext dbContext,
            IValidator<BaseProfileEntity> validator,
            ICompanyImageService companyImageService,
            IVacancieService vacancieService) : base(dbContext, validator, companyImageService)
        {
            _vacancieService = vacancieService;
        }

        [HttpGet]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public override Task<IActionResult> Edit()
        {
            return base.Edit();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public override Task<IActionResult> Edit(Company profile)
        {
            return base.Edit(profile);
        }

        [HttpGet]
        public override async Task<IActionResult> Details(int id)
        {
            var company = await _dbContext.Companies.AsNoTracking()
                .Include(c => c.AppUser)
                .Include(c => c.Vacancies)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            return View(new CompanyDetailsVm()
            {
                Company = company,
                Vacancies = _vacancieService.GetVacancieIndexVmList(company.Vacancies),
            });
        }
    }
}
