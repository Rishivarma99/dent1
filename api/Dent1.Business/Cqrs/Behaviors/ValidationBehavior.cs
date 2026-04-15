using Dent1.Business.Abstractions;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using FluentValidation;

namespace Dent1.Business.Pipeline;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> next)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors.Where(x => x is not null));
        }

        if (failures.Count != 0)
        {
            var details = string.Join("; ", failures.Select(x => x.ErrorMessage));
            throw new AppException(
                Errors.Common.ValidationFailed,
                new Dictionary<string, object> { ["Details"] = details });
        }

        return await next();
    }
}
