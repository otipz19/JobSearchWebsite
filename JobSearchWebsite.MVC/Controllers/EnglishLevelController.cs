using FluentValidation;
using JobSearchWebsite.Data;
using JobSearchWebsite.Data.Entities;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class EnglishLevelController : BaseFilteringEntityController<EnglishLevel>
	{
		public EnglishLevelController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
