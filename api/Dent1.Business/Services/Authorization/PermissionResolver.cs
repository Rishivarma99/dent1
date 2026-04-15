using Dent1.Business.Abstractions;
using Dent1.Data;
using Dent1.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Services;

/// <summary>
/// Computes effective permissions for a user.
/// Formula: GlobalRolePermissions + TenantOverrideAllow - TenantOverrideDeny + UserOverrideAllow - UserOverrideDeny
/// Deny has precedence over Allow.
/// </summary>
public sealed class PermissionResolver : IPermissionResolver
{
    private readonly DentContext _dbContext;

    public PermissionResolver(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<string>> ResolveAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        // Step 1: Get user's role
        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId && u.TenantId == tenantId)
            .Select(u => new { u.RoleId })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Array.Empty<string>();

        // Step 2: Get global role permissions
        var rolePermissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == user.RoleId)
            .Join(
                _dbContext.Permissions.Where(p => p.IsActive),
                rp => rp.PermissionId,
                p => p.Id,
                (rp, p) => p.Code)
            .ToListAsync(cancellationToken);

        // Step 3: Get tenant-specific role overrides
        var tenantRoleOverrides = await _dbContext.TenantRolePermissionOverrides
            .AsNoTracking()
            .Where(tro => tro.TenantId == tenantId && tro.RoleId == user.RoleId)
            .Join(
                _dbContext.Permissions.Where(p => p.IsActive),
                tro => tro.PermissionId,
                p => p.Id,
                (tro, p) => new { p.Code, tro.Effect })
            .ToListAsync(cancellationToken);

        // Step 4: Get user-specific permission overrides
        var userOverrides = await _dbContext.UserPermissionOverrides
            .AsNoTracking()
            .Where(upo => upo.UserId == userId)
            .Join(
                _dbContext.Permissions.Where(p => p.IsActive),
                upo => upo.PermissionId,
                p => p.Id,
                (upo, p) => new { p.Code, upo.Effect })
            .ToListAsync(cancellationToken);

        // Step 5: Compute final permissions (Deny wins)
        var finalPermissions = new HashSet<string>(rolePermissions, StringComparer.Ordinal);

        // Apply tenant role overrides
        foreach (var allow in tenantRoleOverrides.Where(x => x.Effect == PermissionOverrideEffect.Allow))
            finalPermissions.Add(allow.Code);

        foreach (var deny in tenantRoleOverrides.Where(x => x.Effect == PermissionOverrideEffect.Deny))
            finalPermissions.Remove(deny.Code);

        // Apply user-specific overrides
        foreach (var allow in userOverrides.Where(x => x.Effect == PermissionOverrideEffect.Allow))
            finalPermissions.Add(allow.Code);

        foreach (var deny in userOverrides.Where(x => x.Effect == PermissionOverrideEffect.Deny))
            finalPermissions.Remove(deny.Code);

        return finalPermissions.ToArray();
    }
}
