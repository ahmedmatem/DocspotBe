using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bffb2b2c-dd52-4fb1-8a40-28f302958b32", "AQAAAAIAAYagAAAAEJDki3wU4PGkt5k+hePERqVsZEnlyYjPgVltwTBdXkCuiRgjdg0ps63/8XbBJG5vTA==" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentDate_AppointmentTime",
                table: "Appointments",
                columns: new[] { "AppointmentDate", "AppointmentTime" },
                unique: true,
                filter: "[AppointmentStatus] <> 3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentDate_AppointmentTime",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5154f3c4-3246-461a-a00f-c59c1857e9df", "AQAAAAIAAYagAAAAEJNlSmxOEAm2VJP3mIjgh2liQ1GgJCPRDOC95Z1gUREi60yX+xtOw5hWJWeNUTvcwQ==" });
        }
    }
}
