using Data.Entities;
using FluentValidation;

namespace Data.Validators
{
    public class VacancieValidator : AbstractValidator<Vacancie>
    {
        public VacancieValidator()
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
                .Must((v, lsf) => lsf <= v.RightSalaryFork);
            RuleFor(v => v.RightSalaryFork)
                .NotEmpty()
                .GreaterThan(0)
                .LessThanOrEqualTo(int.MaxValue);
        }
    }
}
