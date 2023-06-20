﻿using Data;
using Data.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Exceptions;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Interfaces.Profile;
using Utility.Interfaces.Responds;
using Utility.Toaster;
using Utility.Utilities;
using Utility.ViewModels;

namespace JobSearchWebsite.MVC.Controllers
{
    [Authorize]
    public class JobOfferController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IJobOfferService _jobOfferService;
        private readonly IValidator<JobOffer> _validator;
        private readonly IResumeService _resumeService;
        private readonly ICompanyProfileService _companyProfileService;
        private readonly IVacancieService _vacancieService;

        public JobOfferController(AppDbContext dbContext,
            IJobOfferService jobOfferService,
            IValidator<JobOffer> validator,
            IResumeService resumeService,
            ICompanyProfileService companyProfileService,
            IVacancieService vacancieService)
        {
            _dbContext = dbContext;
            _jobOfferService = jobOfferService;
            _validator = validator;
            _resumeService = resumeService;
            _companyProfileService = companyProfileService;
            _vacancieService = vacancieService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<JobOffer> jobOffers;

            try
            {
                if (User.IsJobseeker())
                {
                    jobOffers = await _jobOfferService.GetJobOffersForJobseeker(User);
                }
                else if (User.IsCompany())
                {
                    jobOffers = await _jobOfferService.GetJobOffersForCompany(User);
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

            return View(_jobOfferService.GetIndexVmList(jobOffers));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int resumeId, int companyId)
        {
            JobOffer jobOffer = await _dbContext.JobOffers.AsNoTracking()
                .Include(o => o.Resume)
                .Include(o => o.Company)
                .Include(o => o.Vacancie)
                .FirstOrDefaultAsync(o => o.ResumeId == resumeId && o.CompanyId == companyId);
            if (jobOffer == null)
            {
                return NotFound();
            }
            if (!(await _resumeService.UserHasAccessTo(User, jobOffer.Resume) ||
                await _companyProfileService.UserOwnsProfile(User, jobOffer.Company)))
            {
                return Forbid();
            }
            return View(_jobOfferService.GetIndexVm(jobOffer));
        }

        [HttpGet]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public async Task<IActionResult> OffersForResume(int id)
        {
            Resume resume = await _dbContext.Resumes.AsNoTracking()
                .Include(r => r.JobOffers)
                    .ThenInclude(o => o.Vacancie)
                .Include(r => r.JobOffers)
                    .ThenInclude(o => o.Company)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (resume == null)
            {
                return NotFound();
            }
            if (!await _resumeService.UserHasAccessTo(User, resume))
            {
                return Forbid();
            }

            return View(_jobOfferService.GetIndexVmList(resume.JobOffers));
        }

        [HttpGet]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> OffersOfVacancie(int id)
        {
            Vacancie vacancie = await _dbContext.Vacancies.AsNoTracking()
                .Include(v => v.JobOffers)
                    .ThenInclude(o => o.Resume)
                .Include(v => v.Company)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (vacancie == null)
            {
                return NotFound();
            }
            if (!await _vacancieService.UserHasAccessTo(User, vacancie))
            {
                return Forbid();
            }

            return View(_jobOfferService.GetIndexVmList(vacancie.JobOffers));
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public async Task<IActionResult> Delete(JobOfferIndexVm fromRequest)
        {
            Resume resume = await _dbContext.Resumes.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == fromRequest.JobOffer.ResumeId);
            if(resume == null)
            {
                return NotFound();
            }
            Company company = await _companyProfileService.GetUserProfile(User);
            if(company == null)
            {
                return NotFound();
            }
            JobOffer offer = await _dbContext.JobOffers
                .FirstOrDefaultAsync(o => o.CompanyId == company.Id && o.ResumeId == resume.Id);

            if (offer == null)
            {
                return NotFound();
            }

            _dbContext.JobOffers.Remove(offer);
            await _dbContext.SaveChangesAsync();
            TempData.Toaster().Success("Job offer was deleted");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public async Task<IActionResult> ChangeStatus(JobOfferIndexVm fromRequest)
        {
            JobOffer offer;
            try
            {
                offer = await _jobOfferService
                    .GetJobOfferForJobseeker(User, fromRequest.JobOffer.ResumeId, fromRequest.JobOffer.CompanyId);
            }
            catch (NoAccessException)
            {
                return Forbid();
            }
            catch
            {
                return NotFound();
            }

            try
            {
                await _jobOfferService.ChangeStatus(offer, fromRequest.JobOffer.Status);
            }
            catch
            {
                TempData.Toaster().Error("This offer has been already considered");
                return RedirectToAction(nameof(Details), new { offer.ResumeId, offer.VacancieId });
            }

            return RedirectToAction(nameof(Details), new { offer.ResumeId, offer.VacancieId });
        }
    }
}
