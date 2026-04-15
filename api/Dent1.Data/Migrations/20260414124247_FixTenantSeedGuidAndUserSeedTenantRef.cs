using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dent1.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTenantSeedGuidAndUserSeedTenantRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Patients",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    DeviceInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantRolePermissionOverrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Effect = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRolePermissionOverrides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionOverrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Effect = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionOverrides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0000-0000-000000000001"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0002-0000-0000-000000000002"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0003-0000-0000-000000000003"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0004-0000-0000-000000000004"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0005-0000-0000-000000000005"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0006-0000-0000-000000000006"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0007-0000-0000-000000000007"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0008-0000-0000-000000000008"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0009-0000-0000-000000000009"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0010-0000-0000-000000000010"),
                column: "TenantId",
                value: new Guid("b1b2c3d4-0001-0000-0000-000000000001"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "Module", "Name" },
                values: new object[,]
                {
                    { new Guid("20000000-0001-0000-0000-000000000001"), "patient.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Patient", "Read Patients" },
                    { new Guid("20000000-0002-0000-0000-000000000001"), "patient.create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Patient", "Create Patient" },
                    { new Guid("20000000-0003-0000-0000-000000000001"), "patient.update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Patient", "Update Patient" },
                    { new Guid("20000000-0004-0000-0000-000000000001"), "appointment.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Appointment", "Read Appointments" },
                    { new Guid("20000000-0005-0000-0000-000000000001"), "appointment.create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Appointment", "Create Appointment" },
                    { new Guid("20000000-0006-0000-0000-000000000001"), "appointment.update", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Appointment", "Update Appointment" },
                    { new Guid("20000000-0007-0000-0000-000000000001"), "prescription.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Prescription", "Read Prescriptions" },
                    { new Guid("20000000-0008-0000-0000-000000000001"), "prescription.create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "Prescription", "Create Prescription" },
                    { new Guid("20000000-0009-0000-0000-000000000001"), "user.read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "User", "Read Users" },
                    { new Guid("20000000-0010-0000-0000-000000000001"), "user.manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", true, "User", "Manage Users" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0001-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System administrator", true, "Admin" },
                    { new Guid("10000000-0002-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dental practitioner", true, "Doctor" },
                    { new Guid("10000000-0003-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Front desk staff", true, "Receptionist" },
                    { new Guid("10000000-0004-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dental assistant", true, "Assistant" },
                    { new Guid("10000000-0005-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Patient user", true, "Patient" }
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name", "UpdatedAt" },
                values: new object[] { new Guid("b1b2c3d4-0001-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Default Clinic", null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("20000000-0001-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0002-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0003-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0004-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0005-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0006-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0007-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0008-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0009-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0010-0000-0000-000000000001"), new Guid("10000000-0001-0000-0000-000000000001") },
                    { new Guid("20000000-0001-0000-0000-000000000001"), new Guid("10000000-0002-0000-0000-000000000001") },
                    { new Guid("20000000-0004-0000-0000-000000000001"), new Guid("10000000-0002-0000-0000-000000000001") },
                    { new Guid("20000000-0006-0000-0000-000000000001"), new Guid("10000000-0002-0000-0000-000000000001") },
                    { new Guid("20000000-0008-0000-0000-000000000001"), new Guid("10000000-0002-0000-0000-000000000001") },
                    { new Guid("20000000-0001-0000-0000-000000000001"), new Guid("10000000-0003-0000-0000-000000000001") },
                    { new Guid("20000000-0002-0000-0000-000000000001"), new Guid("10000000-0003-0000-0000-000000000001") },
                    { new Guid("20000000-0004-0000-0000-000000000001"), new Guid("10000000-0003-0000-0000-000000000001") },
                    { new Guid("20000000-0005-0000-0000-000000000001"), new Guid("10000000-0003-0000-0000-000000000001") },
                    { new Guid("20000000-0001-0000-0000-000000000001"), new Guid("10000000-0004-0000-0000-000000000001") },
                    { new Guid("20000000-0004-0000-0000-000000000001"), new Guid("10000000-0004-0000-0000-000000000001") },
                    { new Guid("20000000-0004-0000-0000-000000000001"), new Guid("10000000-0005-0000-0000-000000000001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_TenantId",
                table: "RefreshTokens",
                columns: new[] { "UserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRolePermissionOverrides_TenantId_RoleId_PermissionId_~",
                table: "TenantRolePermissionOverrides",
                columns: new[] { "TenantId", "RoleId", "PermissionId", "Effect" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionOverrides_UserId_PermissionId_Effect",
                table: "UserPermissionOverrides",
                columns: new[] { "UserId", "PermissionId", "Effect" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TenantRolePermissionOverrides");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "UserPermissionOverrides");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Patients");

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                });
        }
    }
}
