namespace Booking.Domain.Entities;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Canceled,
    Completed
}

public enum AttachmentType
{
    General,        
    MedicalReport, 
    Prescription    
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

    public virtual Doctor Doctor { get; private set; } = null!;
    public virtual Patient Patient { get; private set; } = null!;

    public string? MedicalNotes { get; private set; }
    public string? Diagnosis { get; private set; }
    public string? TreatmentPlan { get; private set; }

    public IReadOnlyCollection<AppointmentAttachment> Attachments => _attachments.AsReadOnly();
    private readonly List<AppointmentAttachment> _attachments = new();

    private Appointment() { }

    public Appointment(Guid doctorId, 
        Guid customerId, 
        DateTime start, 
        DateTime end)
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

    public void AddAttachment(string fileName, string filePath, string fileType, AttachmentType type)
    {
        if (Status == AppointmentStatus.Canceled)
            throw new InvalidOperationException("Cannot add attachments to a canceled appointment.");
        var attachment = new AppointmentAttachment(Id, fileName, filePath, fileType, type);
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

    public void Complete(string diagnosis, string? medicalNotes, string? treatmentPlan)
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed appointments can be completed.");
        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new InvalidOperationException("Diagnosis is required to complete the appointment.");

        MedicalNotes = medicalNotes;
        Diagnosis = diagnosis;
        TreatmentPlan = treatmentPlan;
        Status = AppointmentStatus.Completed;
    }

    public void Reschedule(DateTime newStart, DateTime newEnd)
    {
        if (Status == AppointmentStatus.Canceled)
            throw new InvalidOperationException("Cannot reschedule a canceled appointment.");
        if (newStart >= newEnd)
            throw new ArgumentException("Start time must be before end time.");

        StartTime = newStart;
        EndTime = newEnd;
        Status = AppointmentStatus.Pending; 
    }
}