using Dent1.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data;

public class DentContext : DbContext
{
    public DentContext(DbContextOptions<DentContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<TenantRolePermissionOverride> TenantRolePermissionOverrides => Set<TenantRolePermissionOverride>();
    public DbSet<UserPermissionOverride> UserPermissionOverrides => Set<UserPermissionOverride>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.TenantId).IsRequired();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Phone).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(300);
            entity.Property(u => u.RefreshTokenHash).HasMaxLength(300);
            entity.Property(u => u.RefreshTokenExpiresAt);
            entity.Property(u => u.RefreshTokenCreatedAt);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);
            entity.Property(u => u.IsActive).IsRequired();
            entity.Property(u => u.IsDeleted).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.HasQueryFilter(u => !u.IsDeleted);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.Property(t => t.IsActive).IsRequired();
            entity.Property(t => t.CreatedAt).IsRequired();
        });

        // Configure Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Description).HasMaxLength(500);
            entity.Property(r => r.IsActive).IsRequired();
            entity.Property(r => r.CreatedAt).IsRequired();
        });

        // Configure Permission entity
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Code).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Module).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.IsActive).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.HasIndex(p => p.Code).IsUnique();
        });

        // Configure RolePermission (composite key)
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            entity.HasOne<Role>().WithMany().HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Permission>().WithMany().HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TenantRolePermissionOverride
        modelBuilder.Entity<TenantRolePermissionOverride>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Effect).HasConversion<string>().HasMaxLength(20);
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.HasIndex(t => new { t.TenantId, t.RoleId, t.PermissionId, t.Effect }).IsUnique();
        });

        // Configure UserPermissionOverride
        modelBuilder.Entity<UserPermissionOverride>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Effect).HasConversion<string>().HasMaxLength(20);
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.HasIndex(u => new { u.UserId, u.PermissionId, u.Effect }).IsUnique();
        });

        // Configure RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.TokenHash).IsRequired().HasMaxLength(300);
            entity.Property(r => r.CreatedAtUtc).IsRequired();
            entity.Property(r => r.ExpiresAtUtc).IsRequired();
            entity.Property(r => r.IsUsed).IsRequired();
            entity.HasIndex(r => new { r.UserId, r.TenantId });
            entity.HasIndex(r => r.TokenHash).IsUnique();
        });
    }
}
