using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Domain.Entities;

public class Doctor
{
    public Guid Id { get; init; }
    public string ApplicationUserId { get; private set; } = null!;
    public virtual ApplicationUser ApplicationUser { get; private set; } = null!;
    public Guid SpecialtyId { get; private set; }
    public Specialty Specialty { get; private set; } = null!;
    public double AverageRating { get; private set; }
    public int ReviewsCount { get; private set; }
    public string? Bio { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? ImageUrl { get; private set; }
    public void Deactivate() { IsActive = false; }
    public virtual DoctorScheduleConfig? ScheduleConfig { get; private set; }
    public bool IsActive { get; private set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal ConsultationFee { get; private set; }
    private readonly List<Review> _reviews = new();
    public List<Appointment> Appointments { get; set; } = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private Doctor() { }

    public Doctor(
        string applicationUserId,
        Guid specialtyId,
        bool isActive,
        decimal consultationFee, 
        int experienceYears,
        string? bio,             
        string? imageUrl)
    {
        Id = Guid.NewGuid();
        ApplicationUserId = applicationUserId;
        SpecialtyId = specialtyId;
        IsActive = isActive;

        ConsultationFee = consultationFee;
        ExperienceYears = experienceYears;
        Bio = bio;
        ImageUrl = imageUrl;

        AverageRating = 0;
        ReviewsCount = 0;
    }

    public void UpdateProfile(
        string? bio,
        int experienceYears,
        string? imageUrl,
        bool isActive,
        decimal consultationFee)
    {
        if (experienceYears < 0) 
            throw new ArgumentException("Experience cannot be negative");

        Bio = bio;
        ExperienceYears = experienceYears;
        ImageUrl = imageUrl;
        IsActive = isActive;
        ConsultationFee = consultationFee;
    }

    public void AddReview(Review review)
    {
        if (review == null) throw new ArgumentNullException(nameof(review));

        _reviews.Add(review);

        double totalScore = AverageRating * ReviewsCount;
        totalScore += review.Rating;

        ReviewsCount++;

        AverageRating = Math.Round(totalScore / ReviewsCount, 2);
    }

    public void SetSpecialty(Guid specialtyId)
    {
        if (specialtyId == Guid.Empty)
            throw new ArgumentException("Specialty ID cannot be empty");
        SpecialtyId = specialtyId;
    }

    public void SetImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) 
            throw new ArgumentException("Image URL cannot be empty");
        
        ImageUrl = imageUrl;
    }

    public void SetConsultationFee(decimal fee)
    {
        if (fee < 0) 
            throw new ArgumentException("Fee cannot be negative");
        ConsultationFee = fee;
    }

    public void GoOnVacation()
    {
        IsActive = false;
    }

    public void ReturnFromVacation()
    {
        IsActive = true;
    }
}
