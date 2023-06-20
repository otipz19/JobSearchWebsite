using Data.Entities;
using FluentValidation;

namespace Data.Validators
{
    public class JobOfferValidator : AbstractValidator<JobOffer>
    {
        public JobOfferValidator()
        {
            RuleFor(j => j.Message)
                .NotEmpty()
                .MinimumLength(100)
                .MaximumLength(2000);
        }
    }
}
