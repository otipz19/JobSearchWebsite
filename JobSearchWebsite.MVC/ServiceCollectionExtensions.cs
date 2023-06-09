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
using Data.Entities;
using Utility.ViewModels;
using Utility.Validators;
using Utility.Interfaces.FileUpload.Image;
using Utility.Services.FileUpload.Image;
using Utility.Interfaces.BaseFilterableEntityServices;
using Utility.Services.BaseFilterableEntityServices;
using Utility.Interfaces.FileUpload.Document;
using Utility.Services.FileUpload.Document;
using Laraue.EfCoreTriggers.SqlServer.Extensions;
using Utility.Interfaces.Responds;
using Utility.Services.Responds;
using Utility.Interfaces.FilterServices;
using Utility.Services.FilterServices;
using Utility.Interfaces.Checkbox;
using Utility.Services.Checkbox;
using Utility.Interfaces.OrderServices;
using Utility.Services.OrderServices;
using Microsoft.AspNetCore.Identity.UI.Services;
using Utility.Interfaces.EmailSending;
using Utility.Services.EmailSending;
using Utility.Settings;

namespace JobSearchWebsite.MVC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Localhost"))
                    .UseSqlServerTriggers();
            });
        }

        public static IServiceCollection AddFluentValidators(this IServiceCollection services)
        {
            return services.AddScoped<IValidator<BaseNamedEntity>, BaseNamedEntityValidator>()
                .AddScoped<IValidator<BaseProfileEntity>, BaseProfileEntityValidator>()
                .AddScoped<IValidator<Vacancie>, VacancieValidator>()
                .AddScoped<IValidator<VacancieUpsertVm>, VacancieUpsertVmValidator>()
                .AddScoped<IValidator<ResumeUpsertVm>, ResumeUpsertVmValidator>()
                .AddScoped<IValidator<ResumeDetailsVm>, ResumeDetailsVmValidator>();
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

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddScoped<IJobseekerProfileService, JobseekerProfileService>()
                .AddScoped<ICompanyProfileService, CompanyProfileService>()
                .AddScoped<ICompanyImageService, CompanyImageService>()
                .AddScoped<IJobseekerImageService, JobseekerImageService>()
                .AddScoped<IVacancieService, VacancieService>()
                .AddScoped<IResumeService, ResumeService>()
                .AddScoped<IResumeDocumentService, ResumeDocumentService>()
                .AddScoped<IVacancieRespondService, VacancieRespondService>()
                .AddScoped<IJobOfferService, JobOfferService>()
                .AddScoped<IVacancieFilterService, VacancieFilterService>()
                .AddScoped<IResumeFilterService, ResumeFilterService>()
                .AddScoped<ICheckboxService, CheckboxService>()
                .AddScoped<IVacancieOrderService, VacancieOrderService>()
                .AddScoped<IResumeOrderService, ResumeOrderService>()
                .AddScoped<IEmailSender, EmailSenderService>()
                .AddScoped<IEmailSenderService, EmailSenderService>();
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<MailjetSettings>(configuration.GetSection(MailjetSettings.Section));
        }
    }
}
