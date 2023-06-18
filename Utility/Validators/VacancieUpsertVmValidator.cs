using FluentValidation;
using Utility.ViewModels;

namespace Utility.Validators
{
	public class VacancieUpsertVmValidator : AbstractValidator<VacancieUpsertVm>
	{
        public VacancieUpsertVmValidator()
        {
			RuleFor(v => v.Name)
			   .NotEmpty()
			   .Length(10, 100);
			RuleFor(v => v.Description)
				.NotEmpty()
				.Length(100, 2000);
			RuleFor(v => v.LeftSalaryFork)
				.NotEmpty()
				.GreaterThan(0)
				.Must((v, lsf) => lsf <= v.RightSalaryFork)
				.WithMessage("'From' field should be less than 'To' field");
			RuleFor(v => v.RightSalaryFork)
				.NotEmpty()
				.GreaterThan(0)
				.LessThanOrEqualTo(int.MaxValue);
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
		}
    }
}
