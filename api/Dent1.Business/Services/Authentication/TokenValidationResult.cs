namespace Dent1.Business.Services;

public sealed record TokenValidationResult(
    bool IsValid,
    string? FailureReason = null);