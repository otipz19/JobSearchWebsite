using FluentValidation;
using Utility.ViewModels;

namespace Utility.Validators
{
	public class ResumeDetailsVmValidator : AbstractValidator<ResumeDetailsVm>
	{
        public ResumeDetailsVmValidator()
        {
			RuleFor(v => v.Name)
			   .NotEmpty()
			   .Length(10, 100);
			RuleFor(v => v.Description)
				.NotEmpty()
				.Length(100, 2000);
			RuleFor(v => v.WantedSalary)
				.NotEmpty()
				.GreaterThan(0);
			RuleFor(v => v.SphereId)
				.NotEmpty();
			RuleFor(v => v.SpecializationId)
				.NotEmpty();
			RuleFor(v => v.RemotenessId)
				.NotEmpty();
			RuleFor(v => v.ExperienceLevelId)
				.NotEmpty();
			RuleFor(v => v.EnglishLevelId)
				.NotEmpty();
			RuleFor(v => v.StateId)
				.NotEmpty();
			RuleFor(v => v.CityId)
				.NotEmpty();
		}
    }
}
