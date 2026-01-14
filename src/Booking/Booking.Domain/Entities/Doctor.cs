using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Booking.Domain.Entities;

public class Doctor
{
    public Guid Id { get; init; }
    public string? UserId { get; init; } = null;
    public string Name { get; private set; } = string.Empty;
    public string Lastname {  get; private set; } = string.Empty;
    public string Specialty { get; private set; } = string.Empty;
    public string? Bio { get; private set; }
    public int ExperienceYears { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal ConsultationFee { get; private set; }

    private Doctor() { }

    public Doctor(
        string name, 
        string lastname, 
        string specialty,
        bool isActive,
        string? userId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Lastname = lastname;
        Specialty = specialty;
        UserId = userId;
        IsActive = isActive;
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

    public string GetFullName() => $"{Name} {Lastname}";
    
    public void SetSpecialty(string specialty)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            throw new ArgumentException("Specialty cannot be empty");
        Specialty = specialty;
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
