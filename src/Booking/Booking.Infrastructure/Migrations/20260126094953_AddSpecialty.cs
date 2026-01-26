using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "Doctors");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "Doctors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    SpecialtyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateTimeUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.SpecialtyId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecialtyId",
                table: "Doctors",
                column: "SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specialties_SpecialtyId",
                table: "Doctors",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "SpecialtyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specialties_SpecialtyId",
                table: "Doctors");

            migrationBuilder.DropTable(
                name: "Specialties");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_SpecialtyId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "Doctors",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
