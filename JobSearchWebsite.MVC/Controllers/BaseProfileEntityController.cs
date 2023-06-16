using Data;
using Data.Entities.Base;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Interfaces.FileUpload.Image;
using Utility.Toaster;

namespace JobSearchWebsite.MVC.Controllers
{
    public abstract class BaseProfileEntityController<T> : Controller
        where T : BaseProfileEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly IValidator<BaseProfileEntity> _validator;
        protected readonly ICompanyImageService _companyImageService;

        protected BaseProfileEntityController(AppDbContext dbContext,
            IValidator<BaseProfileEntity> validator,
            ICompanyImageService companyImageService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _companyImageService = companyImageService;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index()
        {
            var profiles = await _dbContext.Set<T>().AsNoTracking().ToListAsync();
            return View(profiles);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Details(int id)
        {
            T profile = await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            T profile = await _dbContext.Set<T>().FirstOrDefaultAsync(p => p.AppUserId == userId);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(T profile)
        {
            var validationResult = await _validator.ValidateAsync(profile);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                return RedirectToAction(nameof(Edit));
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            T toUpdate = await _dbContext.Set<T>().FirstOrDefaultAsync(c => c.AppUserId == userId);
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
                    profile.ImagePath = await _companyImageService.UploadImage(file);
                }
                catch (Exception ex)
                {
                    TempData.Toaster().Error(ex.Message, "Invalid file upload");
                    return RedirectToAction(nameof(Edit));
                }
            }

            toUpdate.Update(profile);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Edited profile successfully");
            return RedirectToAction(nameof(Edit));
        }
    }
}
