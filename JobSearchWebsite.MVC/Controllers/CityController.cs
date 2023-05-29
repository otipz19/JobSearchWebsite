using FluentValidation;
using JobSearchWebsite.Data;
using JobSearchWebsite.Data.Entities;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class CityController : BaseFilteringEntityController<City>
	{
		public CityController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
