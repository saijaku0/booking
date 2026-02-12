namespace Booking.Application.Doctors.Dtos
{
    public record DoctorStatsDto
    {
        public int TotalPatients { get; set; }
        public int CompletedAppointments { get; set; }
        public decimal TotalEarnings { get; set; }
        public string Period { get; set; } = string.Empty;
    }
}
