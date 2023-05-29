using FluentValidation;
using Data;
using Data.Entities;
using Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class CityController : BaseFilteringEntityController<City>
	{
		public CityController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
