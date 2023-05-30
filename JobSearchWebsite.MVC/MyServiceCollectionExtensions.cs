using Data.Validators;
using FluentValidation;
using Data;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Utility.Utilities;
using JobSearchWebsite.MVC.Enums;

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
            return services.AddScoped<IValidator<BaseNamedEntity>, BaseNamedEntityValidator>();
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
    }
}
