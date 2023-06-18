using Data;
using Data.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Interfaces.FileUpload.Document;
using Utility.Toaster;
using Utility.Utilities;
using Utility.ViewModels;

namespace JobSearchWebsite.MVC.Controllers
{
    public class ResumeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IResumeService _resumeService;
        private readonly IValidator<ResumeUpsertVm> _validator;
        private readonly IResumeDocumentService _documentService;

        public ResumeController(AppDbContext dbContext,
            IResumeService resumeService,
            IValidator<ResumeUpsertVm> validator,
            IResumeDocumentService documentService)
        {
            _dbContext = dbContext;
            _resumeService = resumeService;
            _validator = validator;
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var resumes = _resumeService.GetResumeIndexVmList(await _dbContext.Resumes.ToListAsync());
            return View(resumes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Resume resume = await _resumeService.EagerLoadAsNoTracking(id);
            if (resume == null)
            {
                return NotFound();
            }
            return View(resume);
        }

        [HttpGet]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public async Task<IActionResult> Create()
        {
            ResumeUpsertVm viewModel = await _resumeService.GetNewResumeUpsertVm();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public async Task<IActionResult> Create(ResumeUpsertVm viewModel)
        {
            var validationResult = await _validator.ValidateAsync(viewModel);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                await _resumeService.PopulateVmOnValidationFail(viewModel);
                return View(viewModel);
            }

            Resume resume;
            try
            {
                resume = await _resumeService.MapViewModelToEntity(viewModel);
            }
            catch
            {
                return NotFound();
            }

            Jobseeker jobseeker = await _dbContext.Jobseekers
                .FirstOrDefaultAsync(j => j.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (jobseeker == null)
            {
                return NotFound();
            }
            resume.JobseekerId = jobseeker.Id;

            var formFile = HttpContext.Request.Form.Files.FirstOrDefault();
            if (formFile != null)
            {
                try
                {
                    resume.DocumentPath = await _documentService.UploadDoc(formFile);
                }
                catch
                {
                    TempData.Toaster().Error("Invalid file upload");
                    await _resumeService.PopulateVmOnValidationFail(viewModel);
                    return View(viewModel);
                }
            }

            _dbContext.Resumes.Add(resume);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Resume created successfully");
            return RedirectToAction(nameof(Details), new { id = resume.Id });
        }

        [HttpGet]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public async Task<IActionResult> Update(int id)
        {
            Resume toUpdate = await _resumeService.EagerLoadAsNoTracking(id);
            if(toUpdate == null)
            {
                return NotFound();
            }
            if(! await _resumeService.UserHasAccessTo(User, toUpdate))
            {
                return Forbid();
            }
            ResumeUpsertVm viewModel = await _resumeService.MapEntityToViewModel(toUpdate);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public async Task<IActionResult> Update(int id, ResumeUpsertVm viewModel)
        {
            Resume toUpdate = await _resumeService.EagerLoad(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            if (! await _resumeService.UserHasAccessTo(User, toUpdate))
            {
                return Forbid();
            }

            var validationResult = await _validator.ValidateAsync(viewModel);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                TempData.Toaster().ValidationFailed(validationResult);
                await _resumeService.PopulateVmOnValidationFail(viewModel);
                return View(viewModel);
            }

            try
            {
                await _resumeService.MapViewModelToEntity(viewModel, toUpdate);
            }
            catch
            {
                return NotFound();
            }

            var formFile = HttpContext.Request.Form.Files.FirstOrDefault();
            if (formFile != null)
            {
                try
                {
                    toUpdate.DocumentPath = await _documentService.UploadDoc(formFile);
                }
                catch
                {
                    TempData.Toaster().Error("Invalid file upload");
                    await _resumeService.PopulateVmOnValidationFail(viewModel);
                    return View(viewModel);
                }
            }

            _dbContext.Resumes.Update(toUpdate);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Resume updated successfully");
            return RedirectToAction(nameof(Details), new { id = toUpdate.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public async Task<IActionResult> Delete(int id)
        {
            Resume toDelete = await _dbContext.Resumes
                .Include(r => r.Jobseeker)
                .FirstOrDefaultAsync(r => r.Id == id);
            if(toDelete == null) 
            {
                return NotFound();
            }
            if(! await _resumeService.UserHasAccessTo(User, toDelete))
            {
                return Forbid();
            }

            if (!toDelete.DocumentPath.IsNullOrEmpty())
            {
				try
				{
					_documentService.DeleteDoc(toDelete.DocumentPath);
				}
				catch
				{
					return NotFound();
				}
			}

            _dbContext.Resumes.Remove(toDelete);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Resume deleted successfully");
            return RedirectToAction(nameof(Index));
        }
    }
}
