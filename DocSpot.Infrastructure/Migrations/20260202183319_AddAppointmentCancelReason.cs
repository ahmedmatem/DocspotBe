using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentCancelReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "Appointments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "CancelledByAdmin",
                table: "Appointments",
                type: "bit(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f0837667-2b19-4e9e-9c1a-7c20b28985da", "AQAAAAIAAYagAAAAEDx0W08m2NeVIJ3vKalanlEsSCol2l7G3nqFx359UV3a7pyZHA0vmGhQUgLgehlhZw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelledByAdmin",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "12bdd649-0f27-4d7c-8e63-f27ef97a70f5", "AQAAAAIAAYagAAAAECRy7uaW6orNk4ZSGakNB1wb2b+acalKPkAEdMwboXOMvdfSuzaVCVcY0yr5CTGReQ==" });
        }
    }
}
