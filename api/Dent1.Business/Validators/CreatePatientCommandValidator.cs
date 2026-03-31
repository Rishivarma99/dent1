using Dent1.Business.Commands;
using FluentValidation;

namespace Dent1.Business.Validators;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// VALIDATOR FOR CreatePatientCommand
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// This is picked up automatically by ValidationBehavior via DI.
// BusinessBootstrapper calls AddValidatorsFromAssembly() which
// scans this assembly and registers all AbstractValidator<T> classes.
//
// When a POST /patients request arrives:
//   LoggingBehavior → ValidationBehavior (runs THIS) → TransactionBehavior → Handler
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Patient name is required.")
            .MaximumLength(200).WithMessage("Patient name cannot exceed 200 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9\s\-()+]{7,20}$").WithMessage("Phone number must be 7–20 digits and may include +, spaces, hyphens, or parentheses.");
    }
}
