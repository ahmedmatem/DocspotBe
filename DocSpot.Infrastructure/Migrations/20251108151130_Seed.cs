using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6455d797-d71d-4587-963d-0c7c4ac69420", "6455d797-d71d-4587-963d-0c7c4ac69420", "Doctor", "DOCTOR" },
                    { "7b04ce0e-72e0-4319-b73d-741c63bfc5ab", "7b04ce0e-72e0-4319-b73d-741c63bfc5ab", "Admin", "Admin" },
                    { "f64eeef7-bf35-4b8d-9db6-ace187b9f20e", "f64eeef7-bf35-4b8d-9db6-ace187b9f20e", "Patient", "PATIENT" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6a04ce0e-72e0-4319-b73d-741c63bfc5bc", 0, "60a878a7-4b1a-4c72-a15c-b5b2654b95c1", "admin@gmail.com", true, false, null, "admin@gmail.com", "admin@gmail.com", "AQAAAAIAAYagAAAAEKV+7NoSYAAn6lfpg4nxD1hrAfx7bY3twsQawKE0YsoIMp09l9DEsQwbZnH3D1tHAQ==", null, false, "6a04ce0e-72e0-4319-b73d-741c63bfc5bc", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "7b04ce0e-72e0-4319-b73d-741c63bfc5ab", "6a04ce0e-72e0-4319-b73d-741c63bfc5bc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6455d797-d71d-4587-963d-0c7c4ac69420");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f64eeef7-bf35-4b8d-9db6-ace187b9f20e");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "7b04ce0e-72e0-4319-b73d-741c63bfc5ab", "6a04ce0e-72e0-4319-b73d-741c63bfc5bc" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7b04ce0e-72e0-4319-b73d-741c63bfc5ab");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc");
        }
    }
}
