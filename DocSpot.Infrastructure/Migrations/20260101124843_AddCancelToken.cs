using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelTokenExpireAtUtc",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelTokenHash",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAtUtc",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f4e440a6-da0c-4842-99b8-056f688a892f", "AQAAAAIAAYagAAAAEOpXg6c5UmIlEp7a2jXBZNQj4U2uS37P5juIww2y5U0jB7VgCHuaMI52TTlng9IsFA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelTokenExpireAtUtc",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelTokenHash",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1a335f76-22cd-42bb-9d42-48292c0763a8", "AQAAAAIAAYagAAAAEN8RBx6Ie0GixWs+pqr35oLDpeZXF0CQgGIYOxJ0kieJRyyX9ikvtCn0ziULZfOVDA==" });
        }
    }
}
