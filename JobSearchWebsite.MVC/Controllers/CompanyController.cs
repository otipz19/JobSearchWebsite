﻿using Data;
using Data.Entities;
using Data.Entities.Base;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.Utilities;
using Utility.Interfaces.Image;

namespace JobSearchWebsite.MVC.Controllers
{
    public class CompanyController : BaseProfileEntityController<Company>
    {
        public CompanyController(AppDbContext dbContext,
            IValidator<BaseProfileEntity> validator,
            ICompanyImageService companyImageService) : base(dbContext, validator, companyImageService)
        {
        }

        [HttpGet]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public new Task<IActionResult> Edit()
        {
            return base.Edit();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Constants.CompanyPolicy)]
        public new Task<IActionResult> Edit(Company profile)
        {
            return base.Edit(profile);
        }
    }
}
