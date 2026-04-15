namespace Dent1.Data.Repositories.Seeding;

public interface IDatabaseSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class DatabaseSeedService : IDatabaseSeedService
{
    private readonly ITenantSeedService _tenantSeedService;
    private readonly IRoleSeedService _roleSeedService;
    private readonly IPermissionSeedService _permissionSeedService;
    private readonly IRolePermissionSeedService _rolePermissionSeedService;
    private readonly IUserSeedService _userSeedService;
    private readonly IUserPermissionOverrideSeedService _userPermissionOverrideSeedService;
    private readonly IPatientSeedService _patientSeedService;

    public DatabaseSeedService(
        ITenantSeedService tenantSeedService,
        IRoleSeedService roleSeedService,
        IPermissionSeedService permissionSeedService,
        IRolePermissionSeedService rolePermissionSeedService,
        IUserSeedService userSeedService,
        IUserPermissionOverrideSeedService userPermissionOverrideSeedService,
        IPatientSeedService patientSeedService)
    {
        _tenantSeedService = tenantSeedService;
        _roleSeedService = roleSeedService;
        _permissionSeedService = permissionSeedService;
        _rolePermissionSeedService = rolePermissionSeedService;
        _userSeedService = userSeedService;
        _userPermissionOverrideSeedService = userPermissionOverrideSeedService;
        _patientSeedService = patientSeedService;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await _tenantSeedService.SeedAsync(cancellationToken);
        await _roleSeedService.SeedAsync(cancellationToken);
        await _permissionSeedService.SeedAsync(cancellationToken);
        await _rolePermissionSeedService.SeedAsync(cancellationToken);
        await _userSeedService.SeedAsync(cancellationToken);
        await _userPermissionOverrideSeedService.SeedAsync(cancellationToken);
        await _patientSeedService.SeedAsync(cancellationToken);
    }
}
