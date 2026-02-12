public enum Gender
{
    Male,
    Female,
}

namespace Booking.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; init; }

        public string ApplicationUserId { get; private set; } = string.Empty;
        public virtual ApplicationUser? ApplicationUser { get; private set; }
        public virtual ICollection<Review> Reviews { get; private set; } = new List<Review>();

        public DateOnly DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public string? Address { get; private set; }
        public string? PhoneNumber { get; private set; }

        public virtual ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

        private Patient() { }

        public Patient(string applicationUserId, 
            DateOnly dateOfBirth, 
            Gender gender, 
            string? phoneNumber)
        {
            Id = Guid.NewGuid();
            ApplicationUserId = applicationUserId;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            PhoneNumber = phoneNumber;
        }

        public IReadOnlyCollection<Appointment> GetCompletedAppointments()
        {
            return Appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .ToList()
                .AsReadOnly();
        }

        public Appointment GetAppointmentResult(Guid appointmentId)
        {
            var appointment = Appointments.FirstOrDefault(a => a.Id == appointmentId);

            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Completed)
                throw new InvalidOperationException("Appointment is not completed yet.");

            return appointment;
        }
    }

}
