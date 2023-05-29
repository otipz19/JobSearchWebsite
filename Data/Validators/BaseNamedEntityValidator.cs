using FluentValidation;
using JobSearchWebsite.Data.Entities.Base;

namespace Data.Validators
{
	public class BaseNamedEntityValidator : AbstractValidator<BaseNamedEntity>
	{
		public BaseNamedEntityValidator()
		{
			RuleFor(e => e.Name)
				.NotEmpty()
				.Length(1, 100);
		}
	}
}
