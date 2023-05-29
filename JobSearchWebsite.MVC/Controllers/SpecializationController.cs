using FluentValidation;
using Data;
using Data.Entities;
using Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class SpecializationController : BaseFilteringEntityController<Specialization>
	{
		public SpecializationController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
