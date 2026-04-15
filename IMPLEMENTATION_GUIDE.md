# Permission & Authentication System Implementation Guide

## Implemented Components

### 1. Permission System Architecture ✅

#### Entities Created:
- **Role.cs** - Global role templates (Admin, Doctor, Patient, Receptionist, Assistant)
- **Permission.cs** - Global permission master (patient.read, patient.create, etc.)
- **RolePermission.cs** - Global default permissions per role
- **TenantRolePermissionOverride.cs** - Tenant-specific role permission overrides
- **UserPermissionOverride.cs** - User-specific permission overrides
- **PermissionOverrideEffect.cs** - Enum (Allow/Deny)

#### Seeded Data:
- 5 Global Roles (Admin, Doctor, Patient, Receptionist, Assistant)
- 10 Global Permissions
- Default RolePermission mappings for each role

### 2. Authentication System ✅

#### Entities Created:
- **RefreshToken.cs** - Refresh token storage (hashed, with expiry)

#### Updated Entities:
- **User.cs** - Added `RoleId` (GUID reference) and `SecurityStamp` with default value

#### Services Implemented:

**IPermissionResolver & PermissionResolver.cs**
- Computes final effective permissions for a user
- Formula: GlobalRolePermissions + TenantOverrideAllow - TenantOverrideDeny + UserOverrideAllow - UserOverrideDeny
- Deny precedence rule applied

**ITokenAuthStateReader & TokenAuthStateReader.cs**
- Lightweight single-query auth state lookup
- Returns: UserId, TenantId, UserIsActive, TenantIsActive, SecurityStamp
- Used for per-request JWT validation

**TokenValidationService (Updated)**
- Validates JWT claims
- Calls lightweight auth state reader
- Validates:
  - User exists and is active
  - Tenant exists and is active
  - SecurityStamp matches (critical for revocation)
- Per-request auth flow implemented

**JwtTokenService (Updated)**
- Now async: `GenerateAccessTokenAsync(User, CancellationToken)`
- Generates JWT with required claims:
  - `sub`: User ID
  - `nameidentifier`: User ID
  - `tenant_id`: Tenant ID
  - `security_stamp`: Current security stamp
  - `role`: User role
  - `permission`: Repeated claim for each permission (multiple entries)
- Resolves final permissions and adds to JWT

**AuthService (Updated)**
- LoginAsync: Uses async token generation
- RefreshAsync: Uses async token generation
- Both updated to await GenerateAccessTokenAsync

### 3. Dependency Injection ✅

Updated BusinessBootstrapper.cs:
```csharp
services.AddScoped<ITokenAuthStateReader, TokenAuthStateReader>();
services.AddScoped<IPermissionResolver, PermissionResolver>();
```

### 4. Database Configuration ✅

Updated DentContext.cs:
- Added DbSets for all new entities
- Configured composite key for RolePermission
- Configured column conversions (Effect to string)
- Created indices for performance
- Seeded 5 global roles
- Seeded 10 global permissions
- Seeded default RolePermission mappings

## Architecture Details

### Permission Calculation Flow
```
User Request
  ↓
JWT Signature Validated (middleware)
  ↓
Extract Claims: user_id, tenant_id, security_stamp, permissions
  ↓
TokenValidationService validates:
  - User exists & active
  - Tenant exists & active
  - SecurityStamp matches
  ↓
Request Proceeds with Permissions from JWT Claims
```

### Token Generation Flow (Login/Refresh)
```
User Authentication (username/password)
  ↓
User validated and active
  ↓
PermissionResolver computes final permissions
  ↓
GenerateAccessTokenAsync creates JWT with:
  - sub, tenant_id, security_stamp
  - multiple "permission" claims
  ↓
Return AccessToken + RefreshToken (both hashed in DB)
```

### Final Permission Calculation
```
GlobalRolePermissions
+ TenantRolePermissionOverride(Allow)
- TenantRolePermissionOverride(Deny)
+ UserPermissionOverride(Allow)
- UserPermissionOverride(Deny)
= Final Permissions (Deny wins on conflict)
```

## Roles & Default Permissions

### Admin
- patient.read, patient.create, patient.update
- appointment.read, appointment.create, appointment.update
- prescription.read, prescription.create
- user.read, user.manage

### Doctor
- patient.read
- appointment.read, appointment.update
- prescription.create

### Receptionist
- patient.read, patient.create
- appointment.read, appointment.create

### Assistant
- patient.read
- appointment.read

### Patient
- appointment.read

## Key Security Features Implemented

1. **SecurityStamp** - Invalidates old tokens immediately when:
   - Password changed
   - Admin forces re-login
   - User deactivated
   - Critical auth state changes

2. **Refresh Token Management**
   - Hashed before storage
   - Revocable with RevokedAtUtc
   - Support for multiple active tokens (multiple device login)
   - Expiry tracking

3. **Lightweight Auth Validation**
   - Single query per request
   - Loads only necessary auth state
   - No full entity loads

4. **Permission Precedence**
   - Deny always wins
   - Clear override hierarchy
   - Tenant and user-level customization

## Database Migration Required

Run:
```bash
dotnet ef migrations add AddPermissionAndAuthSystem
dotnet ef database update
```

This will create:
- Roles table
- Permissions table
- RolePermissions table
- TenantRolePermissionOverrides table
- UserPermissionOverrides table
- RefreshTokens table
- Update Users table (add RoleId, update SecurityStamp defaults)

## Usage Examples

### Check Permissions in Handler
```csharp
// From claims (runtime check - no DB lookup)
var hasPermission = principal.HasClaim("permission", "patient.create");

// Or via ICurrentUser
var permissions = _currentUser.Permissions;
if (!permissions.Contains("patient.read"))
    throw new UnauthorizedAccessException();
```

### Force User Logout
```csharp
// Change SecurityStamp
user.SecurityStamp = Guid.NewGuid().ToString("N");

// Revoke all refresh tokens
foreach (var token in user.RefreshTokens)
    token.RevokedAtUtc = DateTime.UtcNow;

await _unitOfWork.SaveChangesAsync();
// Next API request fails auth validation (SecurityStamp mismatch)
```

### Grant User Extra Permission
```csharp
var override = new UserPermissionOverride
{
    Id = Guid.NewGuid(),
    UserId = userId,
    PermissionId = appointmentReschedulePermId,
    Effect = PermissionOverrideEffect.Allow,
    CreatedAt = DateTime.UtcNow
};
_dbContext.UserPermissionOverrides.Add(override);

// Update user SecurityStamp to invalidate old tokens
user.SecurityStamp = Guid.NewGuid().ToString("N");
```

## Testing Checklist

- [ ] Run migrations successfully
- [ ] Seed data appears in DB
- [ ] Login generates JWT with permission claims
- [ ] Token validation passes for active user/tenant
- [ ] Token validation fails when SecurityStamp changes
- [ ] Token validation fails when user deactivated
- [ ] Token validation fails when tenant deactivated
- [ ] Permission resolver computes correct permissions
- [ ] Override calculations work (Allow/Deny precedence)
- [ ] Refresh token hashing works
- [ ] Multiple device login (multiple refresh tokens) works
- [ ] Old tokens don't work after password change
