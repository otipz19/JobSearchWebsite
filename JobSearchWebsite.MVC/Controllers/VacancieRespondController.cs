﻿using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Exceptions;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Interfaces.Responds;
using Utility.Toaster;
using Utility.Utilities;
using Utility.ViewModels;

namespace JobSearchWebsite.MVC.Controllers
{
	[Authorize]
	public class VacancieRespondController : Controller
	{
		private readonly AppDbContext _dbContext;
		private readonly IVacancieRespondService _vacancieRespondService;
		private readonly IResumeService _resumeService;
		private readonly IVacancieService _vacancieService;

		public VacancieRespondController(AppDbContext dbContext,
			IVacancieRespondService vacancieRespondService,
			IVacancieService vacancieService,
			IResumeService resumeService)
		{
			_dbContext = dbContext;
			_vacancieRespondService = vacancieRespondService;
			_vacancieService = vacancieService;
			_resumeService = resumeService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			IEnumerable<VacancieRespond> responds;

			try
			{
                if (User.IsJobseeker())
                {
                    responds = await _vacancieRespondService.GetVacancieRespondsForJobseeker(User);
                }
                else if (User.IsCompany())
                {
                    responds = await _vacancieRespondService.GetVacancieRespondsForCompany(User);
                }
                else
                {
                    return Forbid();
                }
            }
			catch
			{
				return NotFound();
			}
			
			return View(_vacancieRespondService.GetIndexVm(responds));
		}

		[HttpGet]
		[Authorize(Policy = Constants.JobseekerPolicy)]
		public async Task<IActionResult> RespondsOfResume(int id)
		{
			Resume resume = await _dbContext.Resumes.AsNoTracking()
				.Include(r => r.VacancieResponds)
					.ThenInclude(respond => respond.Vacancie)
				.FirstOrDefaultAsync(r => r.Id == id);
			if(resume == null)
			{
				return NotFound();
			}
			if(! await _resumeService.UserHasAccessTo(User, resume))
			{
				return Forbid();
			}

			return View(_vacancieRespondService.GetIndexVm(resume.VacancieResponds, resume: resume));
		}

		[HttpGet]
		[Authorize(Policy = Constants.CompanyPolicy)]
		public async Task<IActionResult> RespondsForVacancie(int id)
		{
			Vacancie vacancie = await _dbContext.Vacancies.AsNoTracking()
				.Include(v => v.VacancieResponds)
					.ThenInclude(respond => respond.Resume)
				.FirstOrDefaultAsync(v => v.Id == id);
			if (vacancie == null)
			{
				return NotFound();
			}
			if (!await _vacancieService.UserHasAccessTo(User, vacancie))
			{
				return Forbid();
			}

			return View(_vacancieRespondService.GetIndexVm(vacancie.VacancieResponds, vacancie: vacancie));
		}

		/// <summary>
		/// Jobseeker can totally delete own responds
		/// </summary>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Constants.JobseekerPolicy)]
		public async Task<IActionResult> Delete(VacancieRespondDetailsVm fromRequest)
		{
			Resume resume = await _dbContext.Resumes.AsNoTracking()
				.FirstOrDefaultAsync(r => r.Id == fromRequest.VacancieRespond.ResumeId);
			if(resume == null)
			{
				return NotFound();
			}
			VacancieRespond respond = await _dbContext.VacancieResponds
				.FirstOrDefaultAsync(r => r.VacancieId == fromRequest.VacancieRespond.VacancieId && r.ResumeId == resume.Id);

			if (respond == null)
			{
				return NotFound();
			}
			if(! await _resumeService.UserHasAccessTo(User, resume))
			{
				return Forbid();
			}

			_dbContext.VacancieResponds.Remove(respond);
			await _dbContext.SaveChangesAsync();
			TempData.Toaster().Success("Vacancie respond was deleted");
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Constants.CompanyPolicy)]
		public async Task<IActionResult> ChangeStatus(VacancieRespondDetailsVm fromRequest)
		{
			VacancieRespond respond;
			try
			{
				respond = await _vacancieRespondService
					.GetVacancieRespondForCompany(User, fromRequest.VacancieRespond.ResumeId, fromRequest.VacancieRespond.VacancieId);
			}
			catch(NoAccessException)
			{
				return Forbid();
			}
			catch
			{
				return NotFound();
			}

			try
			{
				await _vacancieRespondService.ChangeStatus(respond, fromRequest.VacancieRespond.Status);
			}
			catch
			{
				TempData.Toaster().Error("This respond has been already considered");
				return RedirectToAction(nameof(Index));
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
