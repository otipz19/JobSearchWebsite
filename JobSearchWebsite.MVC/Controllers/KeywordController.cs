using FluentValidation;
using Data;
using Data.Entities;
using Data.Entities.Base;
using Microsoft.AspNetCore.Mvc;

namespace JobSearchWebsite.MVC.Controllers
{
	public class KeywordController : BaseFilteringEntityController<Keyword>
	{
		public KeywordController(AppDbContext dbContext, IValidator<BaseNamedEntity> validator) 
			: base(dbContext, validator)
		{
		}
	}
}
