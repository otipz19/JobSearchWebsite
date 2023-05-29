using FluentValidation;
using Data;
using Data.Entities;
using Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class EnglishLevelController : BaseFilteringEntityController<EnglishLevel>
	{
		public EnglishLevelController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
