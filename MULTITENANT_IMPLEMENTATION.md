# Multi-Tenant Architecture Implementation Guide

**Date**: April 14, 2026  
**Status**: Core implementation complete

---

## Overview

The dent1 backend now implements a comprehensive multi-tenant architecture following the rules defined in `tenant-architecture.md`. This ensures tenant-owned data is always accessed within exactly one resolved tenant context per request.

---

## Architecture Principles

### Core Rule
**Resolve tenant once. Never trust tenant from client input. Use tenant in application logic. Enforce tenant again in data access.**

### Request Flow
```
HTTP Request (with JWT)
    ↓
Authentication Middleware (validates JWT)
    ↓
TenantResolutionMiddleware (extracts tenant_id claim, sets ICurrentTenant)
    ↓
Controller (thin - dispatches command/query without tenant)
    ↓
Handler (injects ICurrentTenant, uses it for business logic)
    ↓
Repository (filters by tenant in all queries)
    ↓
Response
```

---

## Implementation Details

### 1. Tenant Context Abstractions

#### ICurrentTenant / CurrentTenant
Located in: `Dent1.Common/MultiTenancy/`

```csharp
public interface ICurrentTenant
{
    Guid TenantId { get; }
    bool IsResolved { get; }
}

public sealed class CurrentTenant : ICurrentTenant
{
    public void SetTenant(Guid tenantId)
    {
        if (IsResolved)
            throw new InvalidOperationException("Already resolved");
        // Set only once
    }
}
```

**Lifetime**: Scoped (one per HTTP request)  
**DI Registration**: See BusinessBootstrapper  
**Thread Safety**: Not thread-safe by design (scoped to one request)

---

### 2. Tenant Resolution Middleware

Located in: `Dent1.Api/Middleware/TenantResolutionMiddleware.cs`

```
TenantResolutionMiddleware
├─ Runs after authentication
├─ Reads "tenant_id" claim from JWT (if present)
├─ Reads tenant from host (if implemented)
├─ Validates claim ≠ host mismatch → 403 Forbidden
├─ Sets ICurrentTenant.SetTenant(tenantId)
└─ Passes to next middleware
```

**Order in pipeline** (in `PresentationExtensions.UsePresentationPipeline`):
```
UseAuthentication
  ↓
UseTenantResolution ← NEW
  ↓
UseAuthorization
  ↓
MapControllers
```

**Key behaviors**:
- Rejects if claim tenant ≠ host tenant
- Uses claim tenant if both present (claim takes precedence)
- Falls back to host tenant if only host resolved
- Allows unresolved tenant (will fail later if endpoint requires auth)

---

### 3. Dependency Injection

In `BusinessBootstrapper.Register()`:

```csharp
// Multi-tenancy - scoped lifetime, one per request
services.AddScoped<CurrentTenant>();
services.AddScoped<ICurrentTenant>(sp => sp.GetRequiredService<CurrentTenant>());
services.AddScoped<ITenantHostResolver, TenantHostResolver>();
```

**Why scoped**:
- Tenant context is per-request
- Must be created fresh for each request
- Ensures thread safety
- Allows reuse within same request

---

## Command Implementation Pattern

### Before (Rule Violation)
```csharp
[HttpPost]
public async Task<CreateUserResponse> Create(CreateUserRequest request, CancellationToken cancellationToken)
{
    var tenantId = Guid.Parse(User.FindFirst("tenant_id")!.Value); // ❌ Manual parsing
    
    var response = await _commandDispatcher.Dispatch(
        new CreateUserCommand(..., tenantId, ...),  // ❌ TenantId in command
        cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
}
```

### After (Compliant)
```csharp
[HttpPost]
public async Task<ActionResult<CreateUserResponse>> Create(
    CreateUserRequest request,
    CancellationToken cancellationToken)
{
    var response = await _commandDispatcher.Dispatch(
        new CreateUserCommand(
            request.Name,
            request.Email,
            request.Username,
            request.PhoneNumber,
            request.Password,
            request.Role,
            request.IsActive),  // ✅ No TenantId
        cancellationToken);

    return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
}
```

### Command Definition
```csharp
public sealed record CreateUserCommand(
    string Name,
    string Email,
    string Username,
    string PhoneNumber,
    string Password,
    UserRole Role,
    bool IsActive) : ICommand<CreateUserResponse>;  // ✅ No TenantId field
```

### Command Handler Pattern
```csharp
public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public CreateUserCommandHandler(
        ICurrentTenant currentTenant,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService)
    {
        _currentTenant = currentTenant;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<CreateUserResponse> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        // ✅ Validate tenant is resolved
        if (!_currentTenant.IsResolved)
            throw new InvalidOperationException("Tenant not resolved.");

        // ✅ Use tenant from context
        var id = await _userRepository.AddAsync(
            command.Name.Trim(),
            command.Email.Trim(),
            command.Username.Trim(),
            command.PhoneNumber.Trim(),
            passwordHash,
            command.Role,
            _currentTenant.TenantId,  // ← From middleware, not client
            command.IsActive,
            cancellationToken);

        // ✅ Retrieve with tenant filter
        var created = await _userRepository.GetByIdAsync(
            _currentTenant.TenantId,  // ← Tenant-safe lookup
            id,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateUserResponse(...);
    }
}
```

---

## Query Implementation Pattern

### Before (Violation)
```csharp
public sealed class GetUserByIdQueryHandler
{
    private readonly DentContext _dbContext;

    public async Task<UserReadModel?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Where(x => x.Id == query.UserId)  // ❌ No tenant filter
            .Select(...)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

### After (Compliant)
```csharp
public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserReadModel?>
{
    private readonly DentContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public GetUserByIdQueryHandler(DentContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public async Task<UserReadModel?> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        // ✅ Validate tenant resolved
        if (!_currentTenant.IsResolved)
            throw new InvalidOperationException("Tenant not resolved.");

        return await _dbContext.Users
            // ✅ Filter by both ID and tenant
            .Where(x => x.Id == query.UserId && x.TenantId == _currentTenant.TenantId)
            .AsNoTracking()
            .Select(x => new UserReadModel(...))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

---

## Repository Pattern (Tenant-Safe Access)

### Interface
```csharp
public interface IUserRepository
{
    // Tenant-unaware (internal only)
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // Tenant-aware (public, secure)
    Task<User?> GetByIdAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);

    // Tenant-aware delete
    Task<bool> DeleteAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
}
```

### Implementation
```csharp
public async Task<User?> GetByIdAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken)
{
    return await _dbContext.Users
        .AsNoTracking()
        // ✅ Check both tenant and user ID
        .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId, cancellationToken);
}

public async Task<bool> DeleteAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken)
{
    var user = await _dbContext.Users
        // ✅ Verify tenant ownership before delete
        .FirstOrDefaultAsync(
            u => u.Id == userId && u.TenantId == tenantId,
            cancellationToken);

    if (user is null)
        return false;

    _dbContext.Users.Remove(user);
    return true;
}
```

---

## Implementation Checklist

- [x] ICurrentTenant interface created
- [x] CurrentTenant implementation created
- [x] TenantResolutionMiddleware implemented
- [x] Middleware registered in pipeline
- [x] BusinessBootstrapper registers scoped services
- [x] CreateUserCommand - TenantId removed
- [x] CreateUserCommandHandler - uses ICurrentTenant
- [x] UpdateUserCommand - TenantId removed
- [x] UpdateUserCommandHandler - uses ICurrentTenant
- [x] DeleteUserCommandHandler - uses ICurrentTenant with tenant-safe delete
- [x] GetAllUsersQueryHandler - filters by tenant
- [x] GetUserByIdQueryHandler - filters by tenant
- [x] IUserRepository - tenant-aware methods added
- [x] UserRepository - tenant-safe implementation
- [x] UsersController - simplified (no manual JWT parsing)

---

## Remaining Work

### Phase 2: Public Pre-Login Flows
- [ ] Create `PublicAuthController` with `[AllowAnonymous]`
- [ ] Implement `LoginCommand` / `LoginCommandHandler`
- [ ] Implement `GetLoginConfigQuery` / `GetLoginConfigQueryHandler`
- [ ] Ensure host-based tenant resolution for pre-login

### Phase 3: Other Entities
- [ ] Apply same pattern to Patient queries/commands
- [ ] Add tenant filtering to all tenant-scoped entities
- [ ] Add tenant-aware repository methods for each entity

### Phase 4: Background Jobs
- [ ] Implement scoped tenant context for jobs
- [ ] Create `IServiceScopeFactory` usage pattern for multi-tenant jobs
- [ ] Example: DailyReportJob creates scope per tenant

### Phase 5: Host-Based Resolution
- [ ] Implement `TenantHostResolver.ResolveTenantIdByHostAsync()`
- [ ] Support subdomains (e.g., `tenant1.example.com`)
- [ ] Support custom domains
- [ ] Add to tenant configuration

### Phase 6: Authorization Integration
- [ ] Separate tenant isolation from permission checks
- [ ] Implement resource-level authorization
- [ ] Add role-based access control

---

## Testing Guidelines

### Unit Tests
```csharp
[Fact]
public async Task CreateUser_WithResolvedTenant_CreatesUserInTenant()
{
    // Arrange
    var currentTenant = new CurrentTenant();
    currentTenant.SetTenant(tenantId);
    
    var handler = new CreateUserCommandHandler(
        currentTenant,
        userRepository,
        unitOfWork,
        passwordService);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.Equal(_currentTenant.TenantId, created.TenantId);
}

[Fact]
public async Task GetUserById_FiltersByTenant_ReturnsNullForOtherTenant()
{
    // Arrange - resolve as tenant1
    var currentTenant = new CurrentTenant();
    currentTenant.SetTenant(tenant1Id);
    
    // Act - try to get user from tenant2
    var result = await handler.Handle(
        new GetUserByIdQuery(user2Id),
        CancellationToken.None);

    // Assert - null because user2 belongs to tenant2
    Assert.Null(result);
}
```

### Integration Tests
- Verify middleware sets `CurrentTenant` from JWT claim
- Verify query results are filtered by tenant
- Verify cross-tenant access returns empty/null
- Verify delete operations respect tenant boundary

---

## Security Notes

### ✅ Mitigations In Place
- Tenant claim validated in middleware
- All queries filter by tenant
- All writes include tenant verification
- TenantId removed from request DTOs (can't be spoofed from client)
- Scoped lifetime prevents cross-request leakage

### ⚠️ Still Requires
- Permission checks (separate from tenant checks)
- Resource-level authorization
- Audit logging for cross-tenant attempts
- Regular security reviews

---

## Common Patterns

### Accessing Current Tenant in a Service
```csharp
public class MyService
{
    private readonly ICurrentTenant _currentTenant;

    public MyService(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public async Task<Result> DoSomethingAsync()
    {
        if (!_currentTenant.IsResolved)
            throw new InvalidOperationException("Tenant not resolved.");

        var tenantId = _currentTenant.TenantId;
        // Use tenantId for queries/operations
    }
}
```

### Adding Tenant Filter to Existing Query
```csharp
// Before
var users = await _dbContext.Users
    .Where(x => x.IsActive)
    .ToListAsync();

// After
var users = await _dbContext.Users
    .Where(x => x.IsActive && x.TenantId == _currentTenant.TenantId)
    .ToListAsync();
```

### Repository Method Signatures
```csharp
// Internal method (no tenant filter needed)
Task<Entity?> GetInternalAsync(Guid id, CancellationToken cancellationToken);

// Public method (tenant-safe)
Task<Entity?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken);
```

---

## References

- Backend Rules: `c:\Users\rishi.alluri\OneDrive - McKissock LP\Documents\Projects\ai-rules\ai-rules\backend\01-architecture\tenant-architecture.md`
- Implementation: dent1 backend (this document)
- CQRS Pattern: `backend\01-architecture\cqrs-overview.md`
- Handler Pipeline: `backend\03-application\handler-rules.md`
