using Data.Entities.Base;
using FluentValidation;

namespace Data.Validators
{
	public class BaseFiltereableEntityValidator : AbstractValidator<BaseFilterableEntity>
	{
		public BaseFiltereableEntityValidator()
		{
			RuleFor(e => e.Name)
				.NotEmpty()
				.Length(10, 100);
			RuleFor(e => e.Description)
				.NotEmpty()
				.Length(100, 2000);
		}
	}
}
