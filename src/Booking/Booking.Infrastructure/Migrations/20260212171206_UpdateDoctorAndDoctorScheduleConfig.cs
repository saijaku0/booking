using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorAndDoctorScheduleConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DoctorScheduleConfigs_DoctorId",
                table: "DoctorScheduleConfigs",
                column: "DoctorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorScheduleConfigs_Doctors_DoctorId",
                table: "DoctorScheduleConfigs",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorScheduleConfigs_Doctors_DoctorId",
                table: "DoctorScheduleConfigs");

            migrationBuilder.DropIndex(
                name: "IX_DoctorScheduleConfigs_DoctorId",
                table: "DoctorScheduleConfigs");
        }
    }
}
