using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentStatus",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PublicTokenHash",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpireAtUtc",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1a335f76-22cd-42bb-9d42-48292c0763a8", "AQAAAAIAAYagAAAAEN8RBx6Ie0GixWs+pqr35oLDpeZXF0CQgGIYOxJ0kieJRyyX9ikvtCn0ziULZfOVDA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentStatus",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PublicTokenHash",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TokenExpireAtUtc",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5b1db8e3-2cfa-4b5f-bcb1-5b405a233b7d", "AQAAAAIAAYagAAAAELh0DOQiweJZSHS/NhJsmS3UH31w/qaJVs6NLKGaTsjrP9MIN/AnGljRE3FxB+7lag==" });
        }
    }
}
