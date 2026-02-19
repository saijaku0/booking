using Microsoft.AspNetCore.Identity;

namespace Booking.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? PhotoUrl { get; private set; }
        public string? Address { get; private set; }

        public virtual Patient? PatientProfile { get; private set; }
        public virtual Doctor? DoctorProfile { get; private set; }

        public string FullName => $"{FirstName} {LastName}";

        private ApplicationUser() { }

        public ApplicationUser(
            string firstName,
            string lastName,
            string address)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
        }

        public static ApplicationUser CreateDoctor(
            string email, 
            string firstName, 
            string lastName, 
            string phoneNumber)
        {
            return new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true
            };
        }

        public void UpdatePersonalInfo(
        string name,
        string lastname,
        string? phoneNumber)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrEmpty(lastname))
                throw new ArgumentException("Last name cannot be empty.");

            FirstName = name;
            LastName = lastname;
            PhoneNumber = phoneNumber;
        }

        public static ApplicationUser CreatePatient(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string address)
        {
            return new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Address = address,
                EmailConfirmed = true
            };
        }
    }
}
