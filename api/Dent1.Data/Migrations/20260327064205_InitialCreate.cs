using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dent1.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "CreatedAt", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000001"), new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Rishi Alluri", "9876543210" },
                    { new Guid("a1b2c3d4-0002-0000-0000-000000000002"), new DateTime(2026, 1, 20, 11, 0, 0, 0, DateTimeKind.Utc), "Priya Sharma", "9876543210" },
                    { new Guid("a1b2c3d4-0003-0000-0000-000000000003"), new DateTime(2026, 2, 1, 9, 0, 0, 0, DateTimeKind.Utc), "Amit Patel", "9123456789" },
                    { new Guid("a1b2c3d4-0004-0000-0000-000000000004"), new DateTime(2026, 2, 5, 14, 0, 0, 0, DateTimeKind.Utc), "Sneha Reddy", "9988776655" },
                    { new Guid("a1b2c3d4-0005-0000-0000-000000000005"), new DateTime(2026, 2, 10, 8, 0, 0, 0, DateTimeKind.Utc), "Vikram Singh", "9123456789" },
                    { new Guid("a1b2c3d4-0006-0000-0000-000000000006"), new DateTime(2026, 2, 15, 16, 0, 0, 0, DateTimeKind.Utc), "Ananya Gupta", "9112233445" },
                    { new Guid("a1b2c3d4-0007-0000-0000-000000000007"), new DateTime(2026, 2, 20, 10, 30, 0, 0, DateTimeKind.Utc), "Rajesh Kumar", "9876543210" },
                    { new Guid("a1b2c3d4-0008-0000-0000-000000000008"), new DateTime(2026, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Meena Iyer", "9556677889" },
                    { new Guid("a1b2c3d4-0009-0000-0000-000000000009"), new DateTime(2026, 3, 10, 15, 0, 0, 0, DateTimeKind.Utc), "Suresh Nair", "9445566778" },
                    { new Guid("a1b2c3d4-0010-0000-0000-000000000010"), new DateTime(2026, 3, 15, 9, 30, 0, 0, DateTimeKind.Utc), "Deepa Menon", "9334455667" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
