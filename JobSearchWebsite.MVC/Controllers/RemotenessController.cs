using FluentValidation;
using Data;
using Data.Entities;
using Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
	public class RemotenessController : BaseFilteringEntityController<Remoteness>
	{
		public RemotenessController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
		{
		}
	}
}
