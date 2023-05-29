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
			RuleFor(e => e.Email)
				.EmailAddress();
			RuleFor(e => e.Phone)
				.Must(phone =>
				{
					return int.TryParse(phone, out _);
				})
				.WithMessage("Incorrect phone number");
		}
	}
}
