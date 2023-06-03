﻿using Data.Validators;
using FluentValidation;
using Data;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Utility.Utilities;
using Data.Enums;
using Utility.Interfaces.Profile;
using Utility.Services.Profile;
using Utility.Interfaces.Image;
using Utility.Services.Image;
using Data.Entities;
using JobSearchWebsite.MVC.ViewModels;
using JobSearchWebsite.MVC.Validators;

namespace JobSearchWebsite.MVC
{
    public static class MyServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Localhost"));
            });
        }

        public static IServiceCollection AddFluentValidators(this IServiceCollection services)
        {
            return services.AddScoped<IValidator<BaseNamedEntity>, BaseNamedEntityValidator>()
                .AddScoped<IValidator<BaseProfileEntity>, BaseProfileEntityValidator>()
                .AddScoped<IValidator<Vacancie>, VacancieValidator>()
                .AddScoped<IValidator<VacancieVM>, VacancieVMValidator>();
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
			return services.AddIdentity<IdentityUser, IdentityRole>()
				.AddDefaultTokenProviders()
				.AddDefaultUI()
				.AddEntityFrameworkStores<AppDbContext>().Services;
		}

        public static IServiceCollection AddAuthorizationRoles(this IServiceCollection services)
        {
            return services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.AdminPolicy, policy => policy.RequireRole(AppUserRoleType.Admin.ToString()));
                options.AddPolicy(Constants.JobseekerPolicy, policy => policy.RequireRole(AppUserRoleType.Jobseeker.ToString()));
                options.AddPolicy(Constants.CompanyPolicy, policy => policy.RequireRole(AppUserRoleType.Company.ToString()));
            });
        }

        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            return services.AddScoped<IJobseekerProfileService, JobseekerProfileService>()
                .AddScoped<ICompanyProfileService, CompanyProfileService>()
                .AddScoped<ICompanyImageService, CompanyImageService>()
                .AddScoped<IJobseekerImageService, JobseekerImageService>();
        }
    }
}
