using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHoliday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryCode = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    LocalName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsGlobal = table.Column<bool>(type: "bit(1)", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5154f3c4-3246-461a-a00f-c59c1857e9df", "AQAAAAIAAYagAAAAEJNlSmxOEAm2VJP3mIjgh2liQ1GgJCPRDOC95Z1gUREi60yX+xtOw5hWJWeNUTvcwQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_CountryCode_Date_Name",
                table: "Holidays",
                columns: new[] { "CountryCode", "Date", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0fac180b-b258-4898-b19d-c5bcea6d489d", "AQAAAAIAAYagAAAAEF+Mt8305YyPhQoFhHRcYP2fpEv20sxMOFuIrtmuISx7lLhRsyO6R63A9B+TQ8f3Ag==" });
        }
    }
}
