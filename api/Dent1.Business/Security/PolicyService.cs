namespace Dent1.Business.Security;

/// <summary>
/// Checks business policy rules before state-changing operations.
/// Policy is the third authorization check, applied after permission and scope.
/// Policy validates current resource state against business rules.
/// </summary>
public interface IPolicyService
{
    // This is a marker interface for business policy validation.
    // Implement domain-specific policy services as needed.
    //
    // Example usage in handlers:
    // - PatientPolicyService: cannot prescribe for inactive patient
    // - AppointmentPolicyService: cannot update closed encounter
    // - BillingPolicyService: cannot refund unpaid invoice
}
