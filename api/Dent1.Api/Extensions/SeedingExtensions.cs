using Dent1.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dent1.Api.Extensions;

public static class SeedingExtensions
{
    public static async Task SeedAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var userSeedService = scope.ServiceProvider.GetRequiredService<IUserSeedService>();
        await userSeedService.SeedAsync(cancellationToken);
    }
}