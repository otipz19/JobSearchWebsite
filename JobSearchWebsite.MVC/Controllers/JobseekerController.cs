using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.Utilities;
using Utility.Interfaces.FileUpload.Image;

namespace JobSearchWebsite.MVC.Controllers
{
    public class JobseekerController : BaseProfileEntityController<Jobseeker>
    {
        public JobseekerController(AppDbContext dbContext,
            IValidator<BaseProfileEntity> validator,
            ICompanyImageService companyImageService) : base(dbContext, validator, companyImageService)
        {
        }

        [HttpGet]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public new Task<IActionResult> Edit()
        {
            return base.Edit();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.JobseekerPolicy)]
        public new Task<IActionResult> Edit(Jobseeker profile)
        {
            return base.Edit(profile);
        }
    }
}
