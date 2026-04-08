using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dent1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthFieldsToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenCreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenHash",
                table: "Users",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-1001-0000-0000-000000000001"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000001", null, null, null, "arjun.rao" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-1002-0000-0000-000000000002"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000002", null, null, null, "kavya.iyer" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-2001-0000-0000-000000000003"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000003", null, null, null, "rohan.patient" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-2002-0000-0000-000000000004"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000004", null, null, null, "meera.patient" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-3001-0000-0000-000000000005"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000005", null, null, null, "nikhil.reception" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-3002-0000-0000-000000000006"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000006", null, null, null, "sana.reception" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-4001-0000-0000-000000000007"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000007", null, null, null, "vivek.assistant" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-4002-0000-0000-000000000008"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000008", null, null, null, "neha.assistant" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-5001-0000-0000-000000000009"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000009", null, null, null, "pranav.admin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-5002-0000-0000-000000000010"),
                columns: new[] { "PasswordHash", "PhoneNumber", "RefreshTokenCreatedAt", "RefreshTokenExpiresAt", "RefreshTokenHash", "Username" },
                values: new object[] { "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=", "9000000010", null, null, null, "anika.admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenCreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");
        }
    }
}
