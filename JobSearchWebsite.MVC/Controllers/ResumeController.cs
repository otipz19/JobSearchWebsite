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
using Utility.Interfaces.FilterServices;
using Utility.Interfaces.OrderServices;
using Utility.Interfaces.Profile;
using Utility.Interfaces.Responds;
using Utility.Services.FilterServices;
using Utility.Services.Pagination;
using Utility.Toaster;
using Utility.Utilities;
using Utility.ViewModels;

namespace JobSearchWebsite.MVC.Controllers
{
    public class ResumeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IResumeService _resumeService;
        private readonly IValidator<ResumeUpsertVm> _validatorUpsert;
        private readonly IValidator<ResumeDetailsVm> _validatorDetails;
        private readonly IResumeDocumentService _documentService;
        private readonly ICompanyProfileService _companyProfileService;
        private readonly IVacancieService _vacancieService;
        private readonly IJobOfferService _jobOfferService;
        private readonly IResumeFilterService _filterService;
        private readonly IResumeOrderService _orderService;

        public ResumeController(AppDbContext dbContext,
            IResumeService resumeService,
            IValidator<ResumeUpsertVm> validator,
            IResumeDocumentService documentService,
            ICompanyProfileService companyProfileService,
            IValidator<ResumeDetailsVm> validatorDetails,
            IVacancieService vacancieService,
            IJobOfferService jobOfferService,
            IResumeFilterService filterService,
            IResumeOrderService orderService)
        {
            _dbContext = dbContext;
            _resumeService = resumeService;
            _validatorUpsert = validator;
            _documentService = documentService;
            _companyProfileService = companyProfileService;
            _validatorDetails = validatorDetails;
            _vacancieService = vacancieService;
            _jobOfferService = jobOfferService;
            _filterService = filterService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? id, ResumeIndexListVm fromRequest)
        {
            IQueryable<Resume> resumes = _dbContext.Resumes.Where(r => r.IsPublished);

            if(fromRequest.Filter != null)
            {
                resumes = _filterService.ApplyFilter(resumes, fromRequest.Filter);
            }

            if (fromRequest.Order != null)
            {
                resumes = _orderService.Order(resumes, fromRequest.Order);
            }
            else
            {
                resumes = resumes.OrderByDescending(v => v.PublishedAt);
            }

            fromRequest.CurrentPage = id.HasValue ? id : 1;

            const int PageSize = 10;
            PaginatedList<Resume> paginatedResumes;
            try
            {
                paginatedResumes = await PaginatedList<Resume>
                    .CreateAsync(resumes, fromRequest.CurrentPage.Value, PageSize);
            }
            catch
            {
                return NotFound();
            }

            ResumeIndexListVm viewModel = new()
            {
                Items = _resumeService.GetResumeIndexVmList(paginatedResumes),
                Filter = await _filterService.PopulateFilter(fromRequest.Filter ?? new VacancieResumeFilter()),
                TotalCount = await resumes.CountAsync(),
                CurrentPage = fromRequest.CurrentPage,
                IsPreviousDisabled = !paginatedResumes.HasPreviousPage,
                IsNextDisabled = !paginatedResumes.HasNextPage,
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Resume resume = await _resumeService.EagerLoad(id);
            if (resume == null)
            {
                return NotFound();
            }

            ResumeDetailsVm viewModel = new()
            {
                Resume = resume,
            };
            if (User.IsCompany())
            {
                Company company = await _companyProfileService.GetUserProfile(User);
                if(company == null)
                {
                    return NotFound();
                }
                viewModel.AvailableVacancies = await _dbContext.Vacancies.AsNoTracking()
                    .Where(v => v.CompanyId == company.Id)
                    .ToListAsync();

                resume.CountWatched++;
                _dbContext.Resumes.Update(resume);
                await _dbContext.SaveChangesAsync();
            }

            return View(viewModel);
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
            var validationResult = await _validatorUpsert.ValidateAsync(viewModel);
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

            resume.IsPublished = true;
            resume.PublishedAt = DateTime.Now;

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

            var validationResult = await _validatorUpsert.ValidateAsync(viewModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Offer(ResumeDetailsVm viewModel)
        {
            Company company = await _companyProfileService.GetUserProfile(User);
            if (company == null)
            {
                return NotFound();
            }

            Resume resume = await _dbContext.Resumes.AsNoTracking()
               .FirstOrDefaultAsync(r => r.Id == viewModel.Resume.Id);
            if (resume == null)
            {
                return NotFound();
            }

            if(viewModel.SelectedVacancieId != null)
            {
                Vacancie vacancie = await _dbContext.Vacancies.AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == viewModel.SelectedVacancieId);
                if (vacancie == null)
                {
                    return NotFound();
                }

                if (!await _vacancieService.UserHasAccessTo(User, vacancie))
                {
                    return Forbid();
                }
            }
            if(!viewModel.Message.IsNullOrEmpty())
            {
                var validationResult = await _validatorDetails.ValidateAsync(viewModel);
                if (!validationResult.IsValid)
                {
                    validationResult.AddToModelState(ModelState);
                    TempData.Toaster().ValidationFailed(validationResult);
                    return RedirectToAction(nameof(Details), new { id = resume.Id });
                }
            }
            if(viewModel.SelectedVacancieId == null && viewModel.Message.IsNullOrEmpty())
            {
                TempData.Toaster().Error("You should either choose vacancie or provide a message");
				return RedirectToAction(nameof(Details), new { id = resume.Id });
			}

			try
            {
                await _jobOfferService.CreateJobOffer(resume.Id, company.Id, viewModel.SelectedVacancieId, viewModel.Message);
            }
            catch
            {
                TempData.Toaster().Warning("You've already offered to this resume");
                return RedirectToAction(nameof(Details), new { id = resume.Id });
            }

            TempData.Toaster().Success("Offered to resume successfully");
            return RedirectToAction(nameof(Details), new { id = resume.Id });
        }
    }
}
