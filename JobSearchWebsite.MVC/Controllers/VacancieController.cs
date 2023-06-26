using Data;
using Data.Entities;
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
using Utility.Interfaces.Responds;
using Utility.Interfaces.Profile;
using Utility.Interfaces.FilterServices;
using Utility.Services.FilterServices;
using Utility.Services.Pagination;
using Utility.Interfaces.OrderServices;

namespace JobSearchWebsite.MVC.Controllers
{
	public class VacancieController : Controller
	{
		private readonly AppDbContext _dbContext;
		private readonly IValidator<VacancieUpsertVm> _validator;
		private readonly IVacancieService _vacancieService;
		private readonly IResumeService _resumeService;
		private readonly IVacancieRespondService _vacancieRespondService;
		private readonly IJobseekerProfileService _jobseekerProfileService;
		private readonly IVacancieFilterService _vacancieFilterService;
		private readonly IVacancieOrderService _orderService;

        public VacancieController(AppDbContext dbContext,
            IValidator<VacancieUpsertVm> validator,
            IVacancieService vacancieService,
            IResumeService resumeService,
            IVacancieRespondService vacancieRespondService,
            IJobseekerProfileService jobseekerProfileService,
            IVacancieFilterService vacancieFilter,
            IVacancieOrderService orderService)
        {
            _dbContext = dbContext;
            _validator = validator;
            _vacancieService = vacancieService;
            _resumeService = resumeService;
            _vacancieRespondService = vacancieRespondService;
            _jobseekerProfileService = jobseekerProfileService;
            _vacancieFilterService = vacancieFilter;
            _orderService = orderService;
        }

        [HttpGet]
		public async Task<IActionResult> Index(int? id, VacancieIndexListVm fromRequest)
		{
            IQueryable<Vacancie> vacancies = _dbContext.Vacancies.Where(v => v.IsPublished)
				.Include(v => v.Company)
					.ThenInclude(c => c.AppUser);

            if (fromRequest.Filter != null)
            {
                vacancies = _vacancieFilterService.ApplyFilter(vacancies, fromRequest.Filter);
            }

			if(fromRequest.Order != null)
			{
				vacancies = _orderService.Order(vacancies, fromRequest.Order);
			}
			else
			{
				vacancies = vacancies.OrderByDescending(v => v.PublishedAt);
			}

            fromRequest.CurrentPage = id.HasValue ? id : 1;

            const int PageSize = 10;
			PaginatedList<Vacancie> paginatedVacancies;
			try
			{
				paginatedVacancies = await PaginatedList<Vacancie>
					.CreateAsync(vacancies, fromRequest.CurrentPage.Value, PageSize);
            }
			catch
			{
				return NotFound();
			}

            VacancieIndexListVm viewModel = new()
            {
                Items = _vacancieService.GetVacancieIndexVmList(paginatedVacancies),
                Filter = await _vacancieFilterService.PopulateFilter(fromRequest.Filter ?? new VacancieResumeFilter()),
				TotalCount = await vacancies.CountAsync(),
				CurrentPage = fromRequest.CurrentPage,
				IsPreviousDisabled = !paginatedVacancies.HasPreviousPage,
				IsNextDisabled = !paginatedVacancies.HasNextPage,
            };
            return View(viewModel);
        }

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			Vacancie vacancie = await _vacancieService.EagerLoad(id);
			if (vacancie == null)
			{
				return NotFound();
			}
			var viewModel = new VacancieDetailsVm()
			{
				Vacancie = vacancie
			};
			if (User.IsJobseeker())
			{
				var jobseeker = await _jobseekerProfileService.GetUserProfile(User);
				if (jobseeker == null)
				{
					return NotFound();
				}
				viewModel.AvailableResumes = await _dbContext.Resumes.AsNoTracking()
					.Where(r => r.JobseekerId == jobseeker.Id)
					.ToListAsync();

				vacancie.CountWatched++;
				_dbContext.Vacancies.Update(vacancie);
				await _dbContext.SaveChangesAsync();
			}
			return View(viewModel);
		}

		[HttpGet]
		[Authorize(Policy = Constants.CompanyPolicy)]
		public async Task<IActionResult> Create()
		{
			var viewModel = await _vacancieService.GetNewVacancieUpsertVm();
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Constants.CompanyPolicy)]
		public async Task<IActionResult> Create(VacancieUpsertVm viewModel)
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

			vacancie.IsPublished = true;
			vacancie.PublishedAt = DateTime.Now;

			_dbContext.Vacancies.Add(vacancie);
			await _dbContext.SaveChangesAsync();
			TempData.Toaster().Success("Vacancie was created succesfully");
			return RedirectToAction(nameof(Details), new { id = vacancie.Id });
		}

		[HttpGet]
		[Authorize(Policy = Constants.CompanyPolicy)]
		public async Task<IActionResult> Update(int id)
		{
			Vacancie toUpdate = await _vacancieService.EagerLoadAsNoTracking(id);
			if (toUpdate == null)
			{
				return NotFound();
			}
			if (!await _vacancieService.UserHasAccessTo(User, toUpdate))
			{
				return Forbid();
			}
			return View(await _vacancieService.MapEntityToViewModel(toUpdate));
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Constants.CompanyPolicy)]
		public async Task<IActionResult> Update(int id, VacancieUpsertVm viewModel)
		{
			var validationResult = await _validator.ValidateAsync(viewModel);
			if (!validationResult.IsValid)
			{
				validationResult.AddToModelState(ModelState);
				TempData.Toaster().ValidationFailed(validationResult);
				await _vacancieService.PopulateVmOnValidationFail(viewModel);
				return View(viewModel);
			}

			Vacancie toUpdate = await _vacancieService.EagerLoad(id);
			if (toUpdate == null)
			{
				return NotFound();
			}
			if (!await _vacancieService.UserHasAccessTo(User, toUpdate))
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
			if (!await _vacancieService.UserHasAccessTo(User, toDelete))
			{
				return Forbid();
			}

			_dbContext.Vacancies.Remove(toDelete);
			await _dbContext.SaveChangesAsync();
			TempData.Toaster().Success("Vacancie was deleted successfully");
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Constants.JobseekerPolicy)]
		public async Task<IActionResult> Respond(VacancieDetailsVm viewModel)
		{
			Vacancie vacancie = await _dbContext.Vacancies.AsNoTracking()
				.FirstOrDefaultAsync(v => v.Id == viewModel.Vacancie.Id);
			if (vacancie == null)
			{
				return NotFound();
			}
			Resume resume = await _dbContext.Resumes.AsNoTracking()
				.FirstOrDefaultAsync(r => r.Id == viewModel.SelectedResumeId);
			if (resume == null)
			{
				return NotFound();
			}
			if (!await _resumeService.UserHasAccessTo(User, resume))
			{
				return Forbid();
			}

			try
			{
				await _vacancieRespondService.CreateVacancieRespond(resume.Id, vacancie.Id);
			}
			catch
			{
				TempData.Toaster().Warning("You've already responded to this vacancie");
				return RedirectToAction(nameof(Details), new { id = vacancie.Id });
			}

			TempData.Toaster().Success("Responded to vacancie successfully");
			return RedirectToAction(nameof(Details), new { id = vacancie.Id });
		}
	}
}
