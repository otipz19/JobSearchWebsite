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

namespace JobSearchWebsite.MVC.Controllers
{
    public class JobseekerController : BaseProfileEntityController<Jobseeker>
    {
        private readonly IResumeService _resumeService;

        public JobseekerController(AppDbContext dbContext,
            IValidator<BaseProfileEntity> validator,
            ICompanyImageService companyImageService,
            IResumeService resumeService) : base(dbContext, validator, companyImageService)
        {
            _resumeService = resumeService;
        }

        [HttpGet]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public override Task<IActionResult> Edit()
        {
            return base.Edit();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public override Task<IActionResult> Edit(Jobseeker profile)
        {
            return base.Edit(profile);
        }

        [HttpGet]
        public override async Task<IActionResult> Details(int id)
        {
            var jobseeker = await _dbContext.Jobseekers.AsNoTracking()
                .Include(j => j.AppUser)
                .Include(j => j.Resumes)
                .FirstOrDefaultAsync(j => j.Id == id);
            if (jobseeker == null)
            {
                return NotFound();
            }
            return View(new JobseekerDetailsVm()
            {
                Jobseeker = jobseeker,
                Resumes = _resumeService.GetResumeIndexVmList(jobseeker.Resumes),
            });
        }
    }
}
