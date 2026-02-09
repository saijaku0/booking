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
    public Guid DoctorId { get; private set; } 
    public Guid CustomerId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? MedicalNotes { get; private set; }
    public IReadOnlyCollection<AppointmentAttachment> Attachments => _attachments.AsReadOnly();
    private readonly List<AppointmentAttachment> _attachments = new();

    private Appointment() { }

    public Appointment(Guid doctorId, Guid customerId, DateTime start, DateTime end)
    {
        if (start >= end)
            throw new ArgumentException("Start time must be before end time.");

        Id = Guid.NewGuid();
        DoctorId = doctorId;
        CustomerId = customerId;
        StartTime = start;
        EndTime = end;
        Status = AppointmentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddAttachment(string fileName, string filePath, string fileType)
    {
        if (Status == AppointmentStatus.Canceled)
            throw new InvalidOperationException("Cannot add attachments to a canceled appointment.");
        var attachment = new AppointmentAttachment(Id, fileName, filePath, fileType);
        _attachments.Add(attachment);
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

    public void Complete(string notes)
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed appointments can be completed.");
        if (string.IsNullOrWhiteSpace(notes))
            throw new InvalidOperationException("Medical notes must be provided to complete the appointment.");

        MedicalNotes = notes;
        Status = AppointmentStatus.Completed;
    }
}