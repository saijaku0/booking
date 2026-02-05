using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.CompleteAppointment
{
    public class CompleteAppointmentCommandHandler(
        IBookingDbContext dbContext,
        ICurrentUserService userService,
        UserManager<IdentityUser> userManager)
        : IRequestHandler<CompleteAppointmentCommand>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly ICurrentUserService _userService = userService;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task Handle(
            CompleteAppointmentCommand request,
            CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments
                .FindAsync([request.AppointmentId], cancellationToken)
                ?? throw new NotFoundException("Cannot finde appointment by Id", request.AppointmentId);

            var userId = _userService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not authorized");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new UnauthorizedAccessException("User is not authorized");

            var role = await _userManager.GetRolesAsync(user);
            var isAdmin = role.Contains(Roles.Admin);

            if (!isAdmin)
            {
                var doctor = await _dbContext.Doctors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);

                if (doctor == null || doctor.Id != appointment.DoctorId)
                    throw new UnauthorizedAccessException("You can only complete your own appointments.");
            }

            appointment.Complete(request.MedicalNotes);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
