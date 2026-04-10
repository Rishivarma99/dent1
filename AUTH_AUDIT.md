# Authentication & Authorization Audit

Generated: April 9, 2026

---

## 1. Current Authentication Setup ✅

| Component | Status | Status |
|-----------|--------|--------|
| JWT Bearer | ✅ Configured | Validates signature, audience, issuer, lifetime |
| JWT Issuer | ✅ Configured | From appsettings `Jwt:Issuer` |
| JWT Audience | ✅ Configured | From appsettings `Jwt:Audience` |
| JWT Key | ✅ Configured | From appsettings `Jwt:Key` (HS256) |
| IAuthService | ✅ Exists | Handles login/refresh |
| IJwtTokenService | ✅ Exists | Generates tokens |

---

## 2. Current Authorization Setup ✅

| Component | Status | Details |
|-----------|--------|---------|
| PermissionCodes | ✅ Centralized | `PermissionCodes.cs` with resource.action format |
| UserContext | ✅ Defined | Carries user identity and permissions |
| IPermissionService | ✅ Exists | Checks if user has permission |
| IResourceAccessService | ✅ Exists | Validates scope on resources |
| IPolicyService | ✅ Interface | For domain policy rules |
| AuthorizationBehavior | ✅ Middleware | Enforces permission check in pipeline |
| IAuthorizableRequest | ✅ Interface | Commands/queries declare required permission |

---

## 3. Endpoints Analysis

### AuthController (api/auth)

| Endpoint | Method | Auth | Status |
|----------|--------|------|--------|
| /login | POST | ❌ None | Should be AllowAnonymous |
| /refresh | POST | ❌ None | Should be AllowAnonymous |

**Assessment**: Auth endpoints correctly open.

### PatientsController (api/patients)

| Endpoint | Method | Auth | Status |
|----------|--------|------|--------|
| / | POST | ❌ Missing | Needs `[Authorize]` + permission check |
| / | GET | ❌ Missing | Needs `[Authorize]` + permission check |
| /{id} | GET | ❌ Missing | Needs `[Authorize]` + scope check |
| /search | GET | ❌ Missing | Needs `[Authorize]` + permission check |

**Assessment**: All endpoints unprotected. Needs authorization.

### UsersController (api/users)

| Endpoint | Method | Auth | Status |
|----------|--------|------|--------|
| / | POST | ❌ Missing | Needs `[Authorize]` + admin role |
| / | GET | ❌ Missing | Needs `[Authorize]` + admin role |
| /{id} | GET | ❌ Missing | Needs `[Authorize]` + scope |
| /{id} | PUT | ❌ Missing | Needs `[Authorize]` + admin role |
| /{id} | DELETE | ❌ Missing | Needs `[Authorize]` + admin role |

**Assessment**: All endpoints unprotected. Needs authorization.

### WeatherForecastController (api/weatherforecast)

| Endpoint | Method | Auth | Status |
|----------|--------|------|--------|
| / | GET | ❌ Missing | Test endpoint, should use `[Authorize]` |

**Assessment**: Test endpoint.

---

## 4. Swagger/OpenAPI Support

| Feature | Status | Details |
|---------|--------|---------|
| Swagger UI | ✅ Enabled | `/swagger/index.html` |
| OpenAPI Schema | ✅ Enabled | `/swagger/v1/swagger.json` |
| JWT Security Scheme | ❌ Missing | Not defined in OpenAPI |
| Authorize Button | ❌ Missing | Can't input JWT in Swagger |
| Security Headers | ❌ Missing | Not documented in schema |

**Assessment**: JWT support missing from Swagger. Need to add security scheme.

---

## 5. What's Missing

### High Priority 🔴

1. **Add `[Authorize]` attributes to protected endpoints**
   - PatientsController: All endpoints
   - UsersController: All endpoints except any public endpoints
   - WeatherForecastController: Optional for testing

2. **Configure Swagger JWT Security Scheme**
   - Add OpenAPI security definition
   - Enable "Authorize" button in Swagger UI
   - Allow JWT token input in Swagger

3. **Implement UserContext Resolution**
   - Extract from JWT claims
   - Load effective permissions from database
   - Inject into request context

4. **Apply Commands/Queries Authorization**
   - Make commands inherit `IAuthorizableCommand`
   - Make queries inherit `IAuthorizableQuery`
   - Declare `RequiredPermission` in each

### Medium Priority 🟡

5. **Add Scope Checks to Handlers**
   - Immediate scope validation after resource load
   - Apply scope filters in list queries

6. **Add Policy Checks to Handlers**
   - Domain-specific business rule validation
   - Distinct error messages for policy violations

7. **Authorization Testing**
   - Unit tests for permission service
   - Integration tests for scope validation
   - Test multi-role users
   - Test tenant boundaries

### Low Priority 🟢

8. **Audit Logging**
   - Log all authorization denials
   - Track who attempted what actions

9. **Role-to-Permission Mapping**
   - Create Role, Permission, UserRole, RolePermission tables
   - Seed initial roles with permissions

---

## 6. Implementation Plan

### Phase 1 (Today): Enable Swagger JWT ✅
- Add security scheme to Program.cs
- Enable Authorize button in Swagger UI
- Test with sample token

### Phase 2 (Today): Add [Authorize] to endpoints ✅
- Add attributes to all protected endpoints
- Test scope access restrictions

### Phase 3 (This week): Implement UserContext Resolution
- Create middleware to extract and load UserContext
- Inject into request context

### Phase 4 (This week): Apply to Commands/Queries
- Make sensitive requests implement `IAuthorizableCommand/Query`
- Declare required permissions

### Phase 5 (Next week): Add Scope/Policy Checks
- Implement scope validation in handlers
- Add policy service implementations

---

## 7. Example Authorization Flow (After Implementation)

```
User clicks "Authorize" in Swagger
↓
Enters JWT token
↓
Clicks "Try it out" on protected endpoint
↓
Request sent with "Authorization: Bearer <token>"
↓
JwtBearerAuth validates signature
↓
UserContext loaded from token + database
↓
Endpoint requires [Authorize]
↓
AuthorizationBehavior checks permission
↓
Handler loads resource
↓
Handler validates scope
↓
Handler checks policy
↓
Action executes
↓
Response wrapped in ApiResponse<T>
↓
Swagger displays success response
```

---

## 8. Configuration Checklist

Deploy with these configurations in appsettings.json:

```json
{
  "Jwt": {
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "Key": "your-super-secret-key-at-least-32-characters-long"
  },
  "ConnectionStrings": {
    "DefaultConnection": "PostgreSQL connection string"
  }
}
```

---

## Next Action

Run the implementation checklist below to secure all endpoints.
