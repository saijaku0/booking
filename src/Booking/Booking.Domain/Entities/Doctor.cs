using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Domain.Entities;

public class Doctor
{
    public Guid Id { get; init; }
    public string? UserId { get; init; } = null;
    public string Name { get; private set; } = string.Empty;
    public string Lastname {  get; private set; } = string.Empty;
    public Guid SpecialtyId { get; private set; }
    public Specialty Specialty { get; private set; } = null!;
    public double AverageRating { get; private set; }
    public int ReviewsCount { get; private set; }
    public string? Bio { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal ConsultationFee { get; private set; }
    private readonly List<Review> _reviews = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private Doctor() { }

    public Doctor(
        string name, 
        string lastname, 
        Guid specialtyId,
        bool isActive,
        decimal consultationFee, 
        int experienceYears,
        string userId,
        string? bio,             
        string? imageUrl)
    {
        Id = Guid.NewGuid();
        Name = name;
        Lastname = lastname;
        SpecialtyId = specialtyId;
        UserId = userId;
        IsActive = isActive;

        ConsultationFee = consultationFee;
        ExperienceYears = experienceYears;
        Bio = bio;
        ImageUrl = imageUrl;

        AverageRating = 0;
        ReviewsCount = 0;
    }

    public Doctor(string v1, string v2, Guid specialtyId, bool v3)
    {
        SpecialtyId = specialtyId;
    }

    public void UpdateProfile(
        string name,
        string lastname,
        string? bio,
        int experienceYears,
        string? imageUrl,
        bool isActive,
        decimal consultationFee)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Name cannot be empty");
        if (experienceYears < 0) 
            throw new ArgumentException("Experience cannot be negative");

        Name = name;
        Lastname = lastname;
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

    public string GetFullName() => $"{Name} {Lastname}";
    
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
