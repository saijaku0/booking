namespace Booking.Domain.Entities;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Canceled,
    Completed
}

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid ResourceId { get; private set; } 
    public Guid CustomerId { get; private set; }
    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Appointment() { }

    public Appointment(Guid resourceId, Guid customerId, DateTimeOffset start, DateTimeOffset end)
    {
        if (start >= end)
            throw new ArgumentException("Start time must be before end time.");

        Id = Guid.NewGuid();
        ResourceId = resourceId;
        CustomerId = customerId;
        StartTime = start;
        EndTime = end;
        Status = AppointmentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status == AppointmentStatus.Canceled)
            throw new InvalidOperationException("Cannot confirm canceled appointment.");
        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel()
    {
        Status = AppointmentStatus.Canceled;
    }
}