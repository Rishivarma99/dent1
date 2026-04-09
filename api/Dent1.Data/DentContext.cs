using Dent1.Data.Entities;
using Dent1.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data;

public class DentContext : DbContext
{
    public DentContext(DbContextOptions<DentContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
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

        // Seed 10 patients with some duplicate phone numbers for testing SearchByPhone
        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001"), Name = "Rishi Alluri", Phone = "9876543210", CreatedAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0002-0000-0000-000000000002"), Name = "Priya Sharma", Phone = "9876543210", CreatedAt = new DateTime(2026, 1, 20, 11, 0, 0, DateTimeKind.Utc) },  // duplicate phone
            new Patient { Id = Guid.Parse("a1b2c3d4-0003-0000-0000-000000000003"), Name = "Amit Patel", Phone = "9123456789", CreatedAt = new DateTime(2026, 2, 1, 9, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0004-0000-0000-000000000004"), Name = "Sneha Reddy", Phone = "9988776655", CreatedAt = new DateTime(2026, 2, 5, 14, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0005-0000-0000-000000000005"), Name = "Vikram Singh", Phone = "9123456789", CreatedAt = new DateTime(2026, 2, 10, 8, 0, 0, DateTimeKind.Utc) },   // duplicate phone
            new Patient { Id = Guid.Parse("a1b2c3d4-0006-0000-0000-000000000006"), Name = "Ananya Gupta", Phone = "9112233445", CreatedAt = new DateTime(2026, 2, 15, 16, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0007-0000-0000-000000000007"), Name = "Rajesh Kumar", Phone = "9876543210", CreatedAt = new DateTime(2026, 2, 20, 10, 30, 0, DateTimeKind.Utc) },  // duplicate phone (3rd)
            new Patient { Id = Guid.Parse("a1b2c3d4-0008-0000-0000-000000000008"), Name = "Meena Iyer", Phone = "9556677889", CreatedAt = new DateTime(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0009-0000-0000-000000000009"), Name = "Suresh Nair", Phone = "9445566778", CreatedAt = new DateTime(2026, 3, 10, 15, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0010-0000-0000-000000000010"), Name = "Deepa Menon", Phone = "9334455667", CreatedAt = new DateTime(2026, 3, 15, 9, 30, 0, DateTimeKind.Utc) }
        );
    }
}
