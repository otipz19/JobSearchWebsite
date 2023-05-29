using FluentValidation;
using Data;
using Data.Entities;
using Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class ExperienceLevelController : BaseFilteringEntityController<ExperienceLevel>
	{
		public ExperienceLevelController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
