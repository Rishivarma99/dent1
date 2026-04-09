using FluentValidation;

namespace Dent1.Business.Features.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
