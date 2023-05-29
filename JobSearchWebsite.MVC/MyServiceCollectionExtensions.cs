using Data.Validators;
using FluentValidation;
using Data;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;

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
    }
}
