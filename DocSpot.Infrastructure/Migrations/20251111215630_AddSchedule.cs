using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeekSchedules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SlotLengthMinutes = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeekScheduleIntervals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WeekScheduleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekScheduleIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekScheduleIntervals_WeekSchedules_WeekScheduleId",
                        column: x => x.WeekScheduleId,
                        principalTable: "WeekSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5b1db8e3-2cfa-4b5f-bcb1-5b405a233b7d", "AQAAAAIAAYagAAAAELh0DOQiweJZSHS/NhJsmS3UH31w/qaJVs6NLKGaTsjrP9MIN/AnGljRE3FxB+7lag==" });

            migrationBuilder.CreateIndex(
                name: "IX_WeekScheduleIntervals_WeekScheduleId_Day_Start_End",
                table: "WeekScheduleIntervals",
                columns: new[] { "WeekScheduleId", "Day", "Start", "End" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeekSchedules_StartDate",
                table: "WeekSchedules",
                column: "StartDate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeekScheduleIntervals");

            migrationBuilder.DropTable(
                name: "WeekSchedules");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6a04ce0e-72e0-4319-b73d-741c63bfc5bc",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "60a878a7-4b1a-4c72-a15c-b5b2654b95c1", "AQAAAAIAAYagAAAAEKV+7NoSYAAn6lfpg4nxD1hrAfx7bY3twsQawKE0YsoIMp09l9DEsQwbZnH3D1tHAQ==" });
        }
    }
}
