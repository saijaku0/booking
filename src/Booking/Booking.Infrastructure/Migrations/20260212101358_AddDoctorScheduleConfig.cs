using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorScheduleConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorScheduleConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DayEnd = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LunchStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LunchEnd = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SlotDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    BufferMinutes = table.Column<int>(type: "integer", nullable: false),
                    MinHoursInAdvance = table.Column<int>(type: "integer", nullable: false),
                    MaxDaysInAdvance = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorScheduleConfigs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorScheduleConfigs");
        }
    }
}
