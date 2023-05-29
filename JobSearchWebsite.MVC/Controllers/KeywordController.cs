using FluentValidation;
using JobSearchWebsite.Data;
using JobSearchWebsite.Data.Entities;
using JobSearchWebsite.Data.Entities.Base;
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
