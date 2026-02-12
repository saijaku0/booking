using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Booking.Application.Doctors.Command.CreateDoctor
{
    public class CreateDoctorCommandHandler (
        IBookingDbContext dbContext, 
        UserManager<ApplicationUser> userManager) 
        : IRequestHandler<CreateDoctorCommand, Guid>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Guid> Handle(
            CreateDoctorCommand createDoctor, 
            CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = createDoctor.Email,
                Email = createDoctor.Email,
                FirstName = createDoctor.Name,
                LastName = createDoctor.Lastname,
                EmailConfirmed = true,
            };

            (await _userManager.CreateAsync(user, createDoctor.Password))
                .EnsureSucceeded("CreateDoctor");

            (await _userManager.AddToRoleAsync(user, Roles.Doctor))
                .EnsureSucceeded("AddRole");

            var doctor = new Doctor
            (
                createDoctor.Name,
                createDoctor.Lastname,
                createDoctor.SpecialtyId,
                createDoctor.IsActive,
                createDoctor.ConsultationFee,
                createDoctor.ExperienceYears,
                user.Id,
                createDoctor.Bio,
                createDoctor.ImageUrl
            );

            _dbContext.Doctors.Add(doctor);

            var defaultConfig = new DoctorScheduleConfig(doctor.Id);

            _dbContext.DoctorScheduleConfigs.Add(defaultConfig);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return doctor.Id;
        }
    }
}
