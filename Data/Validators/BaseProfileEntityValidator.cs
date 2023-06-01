using Data.Entities.Base;
using FluentValidation;

namespace Data.Validators
{
	public class BaseProfileEntityValidator : AbstractValidator<BaseProfileEntity>
	{
		public BaseProfileEntityValidator()
		{
			RuleFor(e => e.Name)
				.NotEmpty()
				.Length(5, 100);
			RuleFor(e => e.About)
				.MaximumLength(2000);
		}
	}
}
