using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dent1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "IsDeleted", "Name", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b1c2d3e4-1001-0000-0000-000000000001"), new DateTime(2026, 3, 1, 9, 0, 0, 0, DateTimeKind.Utc), "arjun.rao@dent1.local", true, false, "Dr. Arjun Rao", "Doctor", null },
                    { new Guid("b1c2d3e4-1002-0000-0000-000000000002"), new DateTime(2026, 3, 2, 9, 0, 0, 0, DateTimeKind.Utc), "kavya.iyer@dent1.local", true, false, "Dr. Kavya Iyer", "Doctor", null },
                    { new Guid("b1c2d3e4-2001-0000-0000-000000000003"), new DateTime(2026, 3, 3, 9, 0, 0, 0, DateTimeKind.Utc), "rohan.patient@dent1.local", true, false, "Patient Rohan", "Patient", null },
                    { new Guid("b1c2d3e4-2002-0000-0000-000000000004"), new DateTime(2026, 3, 4, 9, 0, 0, 0, DateTimeKind.Utc), "meera.patient@dent1.local", true, false, "Patient Meera", "Patient", null },
                    { new Guid("b1c2d3e4-3001-0000-0000-000000000005"), new DateTime(2026, 3, 5, 9, 0, 0, 0, DateTimeKind.Utc), "nikhil.reception@dent1.local", true, false, "Receptionist Nikhil", "Receptionist", null },
                    { new Guid("b1c2d3e4-3002-0000-0000-000000000006"), new DateTime(2026, 3, 6, 9, 0, 0, 0, DateTimeKind.Utc), "sana.reception@dent1.local", true, false, "Receptionist Sana", "Receptionist", null },
                    { new Guid("b1c2d3e4-4001-0000-0000-000000000007"), new DateTime(2026, 3, 7, 9, 0, 0, 0, DateTimeKind.Utc), "vivek.assistant@dent1.local", true, false, "Assistant Vivek", "Assistant", null },
                    { new Guid("b1c2d3e4-4002-0000-0000-000000000008"), new DateTime(2026, 3, 8, 9, 0, 0, 0, DateTimeKind.Utc), "neha.assistant@dent1.local", false, false, "Assistant Neha", "Assistant", null },
                    { new Guid("b1c2d3e4-5001-0000-0000-000000000009"), new DateTime(2026, 3, 9, 9, 0, 0, 0, DateTimeKind.Utc), "pranav.admin@dent1.local", true, false, "Admin Pranav", "Admin", null },
                    { new Guid("b1c2d3e4-5002-0000-0000-000000000010"), new DateTime(2026, 3, 10, 9, 0, 0, 0, DateTimeKind.Utc), "anika.admin@dent1.local", true, false, "Admin Anika", "Admin", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
