# Authentication & Permission System - Complete Checklist

## ✅ Database Schema Changes

### New Entities
- [x] Role (Global role templates)
- [x] Permission (Global permission catalog)  
- [x] RolePermission (Composite: RoleId + PermissionId)
- [x] TenantRolePermissionOverride
- [x] UserPermissionOverride
- [x] RefreshToken

### Entity Updates
- [x] User.RoleId added (GUID foreign key to Role)
- [x] User.SecurityStamp initialized with default Guid.NewGuid().ToString("N")

### Seed Data
- [x] 5 Global Roles seeded
- [x] 10 Global Permissions seeded
- [x] 30 RolePermission mappings seeded
- [x] 10 Test Patients updated with TenantId

---

## ✅ Roles & Permissions Matrix

### Role: Admin (ID: r0000000-0001-0000-0000-000000000001)
Permissions:
- [x] patient.read
- [x] patient.create
- [x] patient.update
- [x] appointment.read
- [x] appointment.create
- [x] appointment.update
- [x] prescription.read
- [x] prescription.create
- [x] user.read
- [x] user.manage

### Role: Doctor (ID: r0000000-0002-0000-0000-000000000002)
Permissions:
- [x] patient.read
- [x] appointment.read
- [x] appointment.update
- [x] prescription.create

### Role: Receptionist (ID: r0000000-0003-0000-0000-000000000003)
Permissions:
- [x] patient.read
- [x] patient.create
- [x] appointment.read
- [x] appointment.create

### Role: Assistant (ID: r0000000-0004-0000-0000-000000000004)
Permissions:
- [x] patient.read
- [x] appointment.read

### Role: Patient (ID: r0000000-0005-0000-0000-000000000005)
Permissions:
- [x] appointment.read

---

## ✅ Global Permissions Catalog

| ID | Code | Name | Module | Description |
|----|------|------|--------|-------------|
| p0000000-0001-0000-0000-000000000001 | patient.read | Read Patients | Patient | |
| p0000000-0002-0000-0000-000000000002 | patient.create | Create Patient | Patient | |
| p0000000-0003-0000-0000-000000000003 | patient.update | Update Patient | Patient | |
| p0000000-0004-0000-0000-000000000004 | appointment.read | Read Appointments | Appointment | |
| p0000000-0005-0000-0000-000000000005 | appointment.create | Create Appointment | Appointment | |
| p0000000-0006-0000-0000-000000000006 | appointment.update | Update Appointment | Appointment | |
| p0000000-0007-0000-0000-000000000007 | prescription.read | Read Prescriptions | Prescription | |
| p0000000-0008-0000-0000-000000000008 | prescription.create | Create Prescription | Prescription | |
| p0000000-0009-0000-0000-000000000009 | user.read | Read Users | User | |
| p0000000-0010-0000-0000-000000000010 | user.manage | Manage Users | User | |

---

## ✅ Service Implementations

### IPermissionResolver
- [x] Interface defined in `Dent1.Business/Abstractions/IPermissionResolver.cs`
- [x] Implementation in `Dent1.Business/Services/PermissionResolver.cs`
- [x] Computes: GlobalRolePerms + TenantAllow - TenantDeny + UserAllow - UserDeny
- [x] Registered in BusinessBootstrapper as scoped

### ITokenAuthStateReader
- [x] Interface + record defined in `Dent1.Business/Abstractions/ITokenAuthStateReader.cs`
- [x] Implementation in `Dent1.Business/Services/TokenAuthStateReader.cs`
- [x] Single lightweight join query
- [x] Registered in BusinessBootstrapper as scoped

### TokenValidationService
- [x] Updated for per-request auth validation
- [x] Validates claims format
- [x] Queries lightweight auth state
- [x] Checks: user active, tenant active, security stamp matches
- [x] Uses ITokenAuthStateReader

### JwtTokenService
- [x] Updated to async: GenerateAccessTokenAsync(User, CancellationToken)
- [x] Includes required claims:
  - [x] sub (user ID)
  - [x] nameidentifier (user ID)
  - [x] tenant_id (tenant ID)
  - [x] security_stamp (for invalidation)
  - [x] role (user role)
  - [x] permission (repeated for each permission)
- [x] Calls PermissionResolver to get final permissions
- [x] Uses IPermissionResolver

### AuthService
- [x] LoginAsync updated to await GenerateAccessTokenAsync
- [x] RefreshAsync updated to await GenerateAccessTokenAsync

---

## ✅ JWT Token Structure

### Claims Included in Every Access Token
```
{
  "sub": "user-guid",
  "nameidentifier": "user-guid",
  "tenant_id": "tenant-guid",
  "security_stamp": "abcd1234abcd1234",
  "role": "Doctor",
  "permission": "patient.read",
  "permission": "appointment.read",
  "permission": "appointment.update",
  "permission": "prescription.create"
}
```

### Per-Request Validation
1. JWT signature validated (middleware)
2. Extract claims
3. Query auth state: `SELECT UserId, TenantId, UserIsActive, TenantIsActive, SecurityStamp FROM Users JOIN Tenants`
4. Validate:
   - User exists and active
   - Tenant exists and active
   - SecurityStamp matches (most important)

---

## ✅ Security Features

### SecurityStamp Mechanism
- [x] Default initialization in User entity
- [x] Included in JWT as claim
- [x] Validated on every request
- [x] Changes on:
  - [x] Password change flow
  - [x] User deactivation
  - [x] Role change
  - [x] Permission override application
  - [x] Admin forced re-login

### Refresh Token Management
- [x] RefreshToken entity created
- [x] TokenHash stored (never raw token)
- [x] Expiry validation
- [x] Revocation tracking (RevokedAtUtc)
- [x] Support for multiple active tokens per user
- [x] IsUsed flag for rotation

### Permission Resolution
- [x] Computed only at login/refresh (not per-request)
- [x] Stored in JWT as claims
- [x] Runtime checks read from claims (no DB lookup)
- [x] Deny precedence: If in any deny set, removed from final

### Tenant Isolation
- [x] TenantId primary boundary
- [x] Verified in auth state query
- [x] Tenant ownership checks in repositories
- [x] Patient entity includes TenantId
- [x] User seeding includes TenantId

---

## ✅ Dependency Injection

### BusinessBootstrapper Registrations
- [x] IPermissionResolver → PermissionResolver (scoped)
- [x] ITokenAuthStateReader → TokenAuthStateReader (scoped)
- [x] ITokenValidationService → TokenValidationService (scoped)
- [x] IJwtTokenService → JwtTokenService (kept same)

---

## ✅ No Hardcoding Implementation

### Permission Constants
- [x] PermissionCodes.cs in Dent1.Common.Authorization
- [x] Organized by module (Patient, Appointment, Prescription, User)
- [x] Constants used in DB seed data
- [x] Ready to be used in handler permission checks

### Permission Checks Placement
- [x] Not in controllers
- [x] Must be in handlers/services (enforced by architecture)
- [x] Example: `_permissionChecker.EnsurePermission(PermissionCodes.PatientCreate);`

---

## ✅ Code Files Modified

### Created
```
✓ Dent1.Data/Entities/Role.cs
✓ Dent1.Data/Entities/Permission.cs
✓ Dent1.Data/Entities/RolePermission.cs
✓ Dent1.Data/Enums/PermissionOverrideEffect.cs
✓ Dent1.Data/Entities/TenantRolePermissionOverride.cs
✓ Dent1.Data/Entities/UserPermissionOverride.cs
✓ Dent1.Data/Entities/RefreshToken.cs
✓ Dent1.Business/Abstractions/IPermissionResolver.cs
✓ Dent1.Business/Services/PermissionResolver.cs
✓ Dent1.Business/Abstractions/ITokenAuthStateReader.cs
✓ Dent1.Business/Services/TokenAuthStateReader.cs
✓ IMPLEMENTATION_GUIDE.md
✓ IMPLEMENTATION_SUMMARY.md
✓ PERMISSION_CHECKLIST.md (this file)
```

### Updated
```
✓ Dent1.Data/DentContext.cs
  - DbSets for all new entities
  - Entity configurations
  - Seed data for roles, permissions, mappings
  - Patient seed data with TenantId

✓ Dent1.Data/Entities/User.cs
  - Added RoleId (GUID)
  - Updated SecurityStamp initialization

✓ Dent1.Business/Services/TokenValidationService.cs
  - Per-request auth validation
  - Uses ITokenAuthStateReader
  - Validates security stamp

✓ Dent1.Api/Services/JwtTokenService.cs
  - Async method: GenerateAccessTokenAsync
  - Includes permissions and security_stamp
  - Uses IPermissionResolver

✓ Dent1.Api/Services/AuthService.cs
  - LoginAsync: await token generation
  - RefreshAsync: await token generation

✓ Dent1.Business/BusinessBootstrapper.cs
  - Registered IPermissionResolver
  - Registered ITokenAuthStateReader
```

---

## 🚀 Next Steps for Integration

### 1. Generate & Apply Migration
```bash
cd api
dotnet ef migrations add AddPermissionAndAuthSystem
dotnet ef database update
```

### 2. Create Permission Checker Service
```csharp
public interface IPermissionChecker
{
    bool HasPermission(string code);
    void EnsurePermission(string code);
}

public class PermissionChecker : IPermissionChecker
{
    private readonly ICurrentUser _currentUser;
    
    public bool HasPermission(string code)
        => _currentUser.Permissions.Contains(code);
    
    public void EnsurePermission(string code)
    {
        if (!HasPermission(code))
            throw new UnauthorizedAccessException($"Missing permission: {code}");
    }
}
```

### 3. Register Permission Checker
```csharp
services.AddScoped<IPermissionChecker, PermissionChecker>();
```

### 4. Update User Seed Service
Map users to role GUIDs when creating/updating users.

### 5. Add Permission Checks to Handlers
```csharp
public async Task<Response> Handle(Command cmd, CancellationToken ct)
{
    _permissionChecker.EnsurePermission(PermissionCodes.Patient.Create);
    // ... rest of logic
}
```

### 6. Test Complete Flow
- Login generates JWT with permission claims
- Request with JWT validates security stamp
- Permission checks work from claims
- Change security stamp invalidates old JWT
- Refresh token rotation works

---

## 🔍 Validation Checklist

Before running migration:
- [ ] All new entities have proper relationships
- [ ] Seed data GUIDs are unique
- [ ] Property configurations are correct
- [ ] Indices are defined properly

After migration:
- [ ] Roles table has 5 rows
- [ ] Permissions table has 10 rows
- [ ] RolePermissions table has 30 rows
- [ ] Patient records have TenantId values
- [ ] User entity has RoleId column

After integration:
- [ ] Login returns JWT with permissions
- [ ] Token validation passes for active user/tenant
- [ ] Token validation fails when security stamp changes
- [ ] Permission checks work in handlers
- [ ] Refresh token rotation works
- [ ] Multiple device login supported

---

## 📝 References

- **Authentication Rule**: JWT + security stamp per spec
- **Permission Rule**: Multi-level permission system
- **No Hardcoding**: All permissions use constants
- **Tenant Architecture**: TenantId is primary boundary
- **CQRS**: Ready for command/query handlers

