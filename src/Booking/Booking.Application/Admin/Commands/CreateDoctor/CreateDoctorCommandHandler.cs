using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Booking.Application.Admin.Commands.CreateDoctor
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
                createDoctor.Specialty,
                true,
                user.Id
            );

            _dbContext.Doctors.Add(doctor);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return doctor.Id;
        }
    }
}
