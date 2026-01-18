using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleExclusion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleExclusions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ExclusionType = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    End = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "bit(1)", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleExclusions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "12bdd649-0f27-4d7c-8e63-f27ef97a70f5", "AQAAAAIAAYagAAAAECRy7uaW6orNk4ZSGakNB1wb2b+acalKPkAEdMwboXOMvdfSuzaVCVcY0yr5CTGReQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleExclusions_Date_Start_End_ExclusionType",
                table: "ScheduleExclusions",
                columns: new[] { "Date", "Start", "End", "ExclusionType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleExclusions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bffb2b2c-dd52-4fb1-8a40-28f302958b32", "AQAAAAIAAYagAAAAEJDki3wU4PGkt5k+hePERqVsZEnlyYjPgVltwTBdXkCuiRgjdg0ps63/8XbBJG5vTA==" });
        }
    }
}
