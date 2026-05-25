using FluentValidation;

namespace Dent1.Business.Features.Authentication.Queries.GetCurrentAuthUser;

public sealed class GetCurrentAuthUserQueryValidator : AbstractValidator<GetCurrentAuthUserQuery>
{
    public GetCurrentAuthUserQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
