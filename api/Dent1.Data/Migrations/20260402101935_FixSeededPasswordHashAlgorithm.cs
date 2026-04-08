using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dent1.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSeededPasswordHashAlgorithm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-1001-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-1002-0000-0000-000000000002"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-2001-0000-0000-000000000003"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-2002-0000-0000-000000000004"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-3001-0000-0000-000000000005"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-3002-0000-0000-000000000006"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-4001-0000-0000-000000000007"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-4002-0000-0000-000000000008"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-5001-0000-0000-000000000009"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-5002-0000-0000-000000000010"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.MIegasu+/slhGIJg+rHKzOvomQCZZPWMRU1+vj0STaQ=");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-1001-0000-0000-000000000001"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-1002-0000-0000-000000000002"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-2001-0000-0000-000000000003"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-2002-0000-0000-000000000004"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-3001-0000-0000-000000000005"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-3002-0000-0000-000000000006"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-4001-0000-0000-000000000007"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-4002-0000-0000-000000000008"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-5001-0000-0000-000000000009"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-5002-0000-0000-000000000010"),
                column: "PasswordHash",
                value: "100000.JT5VEhzJuyrmVT1JUfTzTg==.a3CIHvn/ArSNP9NoDP75Fxx89PxtN07O0uCXfS4BXdc=");
        }
    }
}
