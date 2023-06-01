using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Interfaces.Image;
using Utility.Toaster;

namespace JobSearchWebsite.MVC.Controllers
{
    public class CompanyController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidator<BaseProfileEntity> _validator;
        private readonly ICompanyImageService _companyImageService;

        public CompanyController(AppDbContext dbContext,
            IValidator<BaseProfileEntity> validator,
            ICompanyImageService companyImageService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _companyImageService = companyImageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var companies = await _dbContext.Companies.AsNoTracking().ToListAsync();
            return View(companies);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Company company = await _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Company company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.AppUserId == userId);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Company company)
        {
            var validationResult = await _validator.ValidateAsync(company);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().Error("Invalid input");
                return RedirectToAction(nameof(Edit));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Company toUpdate = await _dbContext.Companies.FirstOrDefaultAsync(c => c.AppUserId == userId);
            if (toUpdate == null)
            {
                return NotFound();
            }

            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            //If image was edited
            if (file != null)
            {
                try
                {
                    company.ImagePath = await _companyImageService.UploadImage(file);
                }
                catch
                {
                    TempData.Toaster().Error("Invalid file upload");
                    return RedirectToAction(nameof(Edit));
                }
            }        

            toUpdate.Update(company);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Edited profile successfully");
            return RedirectToAction(nameof(Edit));
        }
    }
}
