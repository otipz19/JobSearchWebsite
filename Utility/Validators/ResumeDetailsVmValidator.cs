using FluentValidation;
using Utility.ViewModels;

namespace Utility.Validators
{
    public class ResumeDetailsVmValidator : AbstractValidator<ResumeDetailsVm>
    {
        public ResumeDetailsVmValidator()
        {
            RuleFor(j => j.Message)
                .NotEmpty()
                .MinimumLength(100)
                .MaximumLength(2000);
        }
    }
}
