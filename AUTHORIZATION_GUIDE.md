# Authorization Implementation Guide

This guide shows how to implement the permission + scope + policy authorization pattern in dent1.

---

## 1. Declare Required Permission in Commands/Queries

```csharp
using Dent1.Business.Abstractions;
using Dent1.Common.Authorization;

public sealed record CreatePatientCommand(string Name, string Phone) 
    : IAuthorizableCommand
{
    public string RequiredPermission => PermissionCodes.PatientCreate;
}

public sealed record GetPatientQuery(Guid PatientId) 
    : IAuthorizableQuery
{
    public string RequiredPermission => PermissionCodes.PatientRead;
}
```

---

## 2. Authorization Pipeline Order

Requests follow this strict order:

1. **Authenticate** - Verify JWT is valid
2. **Build context** - Load UserContext from database
3. **Resolve permissions** - Get user's effective permissions
4. **Check permission** - AuthorizationBehavior validates permission code
5. **Check scope** - Handler loads resource and validates scope
6. **Check policy** - Handler validates business rules
7. **Execute** - Business logic runs

---

## 3. Implement Scope Check in Handlers

```csharp
using Dent1.Business.Security;
using Dent1.Common.Authorization;
using Dent1.Common.Exceptions;
using Dent1.Common.Errors;
using Dent1.Data.Interfaces;

public sealed class GetPatientQueryHandler : IQueryHandler<GetPatientQuery, PatientDto>
{
    private readonly IPatientRepository _repository;
    private readonly IResourceAccessService _accessService;

    public GetPatientQueryHandler(
        IPatientRepository repository,
        IResourceAccessService accessService)
    {
        _repository = repository;
        _accessService = accessService;
    }

    public async Task<PatientDto> Handle(GetPatientQuery query, CancellationToken cancellationToken)
    {
        // Step 1: Load resource
        var patient = await _repository.GetAsync(query.PatientId, cancellationToken);
        if (patient == null)
            throw new AppException(Errors.Patient.NotFound, 
                new Dictionary<string, object> { { "PatientId", query.PatientId } });

        // Step 2: Check scope (must be immediate after loading)
        var userContext = query.UserContext; // Set by controller/middleware
        if (!_accessService.CanAccess(userContext, PermissionCodes.PatientRead, patient.TenantId, patient.AssignedDoctorId))
        {
            throw new AppException(Errors.Auth.ScopeDenied);
        }

        // Step 3: Check policy (if needed)
        // if (!patient.IsActive)
        //     throw new AppException(Errors.Auth.PolicyViolation);

        return patient.ToDto();
    }
}
```

---

## 4. List Queries Must Apply Scope at DB Level

```csharp
public sealed class GetMyPatientsQuery : IAuthorizableQuery
{
    public string RequiredPermission => PermissionCodes.PatientRead;
}

public sealed class GetMyPatientsQueryHandler : IQueryHandler<GetMyPatientsQuery, List<PatientDto>>
{
    private readonly IPatientRepository _repository;

    public GetMyPatientsQueryHandler(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PatientDto>> Handle(GetMyPatientsQuery query, CancellationToken cancellationToken)
    {
        var userContext = query.UserContext;
        
        // Scope is applied at DB level - only return assigned patients
        var patients = await _repository.GetAssignedToDoctorAsync(userContext.UserId, cancellationToken);
        
        return patients.Select(p => p.ToDto()).ToList();
    }
}
```

---

## 5. Error Messages Are Distinct

Different denial reasons return different error codes:

- `PermissionDenied` - User lacks required permission
- `ScopeDenied` - Permission exists but not for this resource
- `PolicyViolation` - Permission and scope OK but business state invalid
- `Unauthorized` - User not authenticated

---

## 6. Logging

Authorization denials are logged with full context:

```
AuthorizationBehavior: Permission denied. UserId: guid, TenantId: guid, Permission: patient.read
```

This helps debugging and audits.

---

## 7. Testing Authorization

For each protected action, test:

1. **Allow case** - Permission + Scope + Policy valid → Success
2. **Permission fail** - Missing permission → PermissionDenied
3. **Scope fail** - Permission valid but scope invalid → ScopeDenied
4. **Policy fail** - Permission/Scope valid but policy invalid → PolicyViolation

---

## Summary

Follow this pattern for every protected handler:

```csharp
// 1. Load resource
var entity = await repository.GetAsync(...);

// 2. Check scope immediately after load
var scopes = userContext.GetScopes(PermissionCodes.X);
if (!resourceAccessService.CanAccess(userContext, permission, tenantId, ownerId))
    throw denied;

// 3. Check policy before mutations
if (!policyService.Allow(...))
    throw policy_error;

// 4. Execute business logic
```
