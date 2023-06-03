using FluentValidation;
using Data;
using Data.Entities;
using Data.Entities.Base;

namespace JobSearchWebsite.MVC.Controllers
{
    public class StateController : BaseFilteringEntityController<State>
    {
        public StateController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) : base(dbContext, validator)
        {
        }
    }
}
