namespace Dent1.Common.Errors;

public static class Errors
{
    public static class Patient
    {
        public static readonly ErrorDefinition NotFound = new()
        {
            Code = "PATIENT_NOT_FOUND",
            MessageTemplate = "Patient with id {PatientId} not found",
            StatusCode = 404
        };

        public static readonly ErrorDefinition Duplicate = new()
        {
            Code = "PATIENT_DUPLICATE",
            MessageTemplate = "A patient with the same name and phone already exists",
            StatusCode = 409
        };

        public static readonly ErrorDefinition InvalidSearchPhone = new()
        {
            Code = "PATIENT_PHONE_REQUIRED",
            MessageTemplate = "Phone is required for patient search",
            StatusCode = 400
        };
    }

    public static class User
    {
        public static readonly ErrorDefinition NotFound = new()
        {
            Code = "USER_NOT_FOUND",
            MessageTemplate = "User with id {UserId} not found",
            StatusCode = 404
        };

        public static readonly ErrorDefinition UsernameExists = new()
        {
            Code = "USER_USERNAME_EXISTS",
            MessageTemplate = "Username already exists",
            StatusCode = 409
        };

        public static readonly ErrorDefinition PhoneExists = new()
        {
            Code = "USER_PHONE_EXISTS",
            MessageTemplate = "Phone number already exists",
            StatusCode = 409
        };
    }

    public static class Auth
    {
        public static readonly ErrorDefinition Unauthorized = new()
        {
            Code = "UNAUTHORIZED",
            MessageTemplate = "Authentication is required to access this resource",
            StatusCode = 401
        };

        public static readonly ErrorDefinition PermissionDenied = new()
        {
            Code = "PERMISSION_DENIED",
            MessageTemplate = "You do not have permission: {Permission}",
            StatusCode = 403
        };

        public static readonly ErrorDefinition ScopeDenied = new()
        {
            Code = "SCOPE_DENIED",
            MessageTemplate = "You cannot access this resource",
            StatusCode = 403
        };

        public static readonly ErrorDefinition PolicyViolation = new()
        {
            Code = "POLICY_VIOLATION",
            MessageTemplate = "This action is not allowed in the current state",
            StatusCode = 422
        };

        public static readonly ErrorDefinition InsufficientPermission = new()
        {
            Code = "INSUFFICIENT_PERMISSION",
            MessageTemplate = "Insufficient permissions for operation: {Operation}",
            StatusCode = 403
        };
    }

    public static class Common
    {
        public static readonly ErrorDefinition ValidationFailed = new()
        {
            Code = "VALIDATION_FAILED",
            MessageTemplate = "Validation failed: {Details}",
            StatusCode = 400
        };

        public static readonly ErrorDefinition InternalServerError = new()
        {
            Code = "INTERNAL_SERVER_ERROR",
            MessageTemplate = "Something went wrong",
            StatusCode = 500
        };
    }
}
