using FluentValidation;

namespace Dent1.Business.Features.Patients.Queries.SearchPatientsByPhone;

public sealed class SearchPatientsByPhoneQueryValidator : AbstractValidator<SearchPatientsByPhoneQuery>
{
    public SearchPatientsByPhoneQueryValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(20);
    }
}
