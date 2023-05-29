using FluentValidation;
using JobSearchWebsite.Data;
using JobSearchWebsite.Data.Entities;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class ExperienceLevelController : BaseFilteringEntityController<ExperienceLevel>
	{
		public ExperienceLevelController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
