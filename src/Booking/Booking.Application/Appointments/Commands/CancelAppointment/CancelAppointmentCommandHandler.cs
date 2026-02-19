using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.CancelAppointment
{
    public class CancelAppointmentCommandHandler(
            IBookingDbContext bookingDbContext,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager)
        : IRequestHandler<CancelAppointmentCommand>
    {
        private readonly IBookingDbContext _bookingDbContext = bookingDbContext;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task Handle(
            CancelAppointmentCommand command,
            CancellationToken cancellationToken)
        {
            var appointment = await _bookingDbContext.Appointments
                .FindAsync([command.Id], cancellationToken)
                ?? throw new KeyNotFoundException($"Appointment with id {command.Id} was not found.");

            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException("User not authenticated.");

            bool isMyAppointment = appointment.Patient.ApplicationUserId == currentUserId
                    || appointment.Doctor.ApplicationUserId == currentUserId;

            if (!isMyAppointment)
                throw new ForbiddenAccessException("You cannot cancel someone else's appointment.");

            var user = await _userManager.FindByIdAsync(currentUserId)
                ?? throw new UnauthorizedAccessException("User not found.");

            var roles = (await _userManager.GetRolesAsync(user)).ToHashSet();

            await EnsureUserAuthorizedToCancelAsync(
                appointment,
                currentUserId,
                roles,
                cancellationToken);

            appointment.Cancel();
            await _bookingDbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task EnsureUserAuthorizedToCancelAsync(
            Appointment appointment,
            string currentUserId,
            HashSet<string> roles,
            CancellationToken cancellationToken)
        {
            if (roles.Contains(Roles.Admin))
                return;

            if (roles.Contains(Roles.Patient))
            {
                if (!Guid.TryParse(currentUserId, out var currentUserGuid))
                    throw new UnauthorizedAccessException("Invalid user id format.");

                if (appointment.PatientId != currentUserGuid)
                    throw new UnauthorizedAccessException("You cannot delete someone else's appointment.");

                return;
            }

            if (roles.Contains(Roles.Doctor))
            {
                var doctor = await _bookingDbContext.Doctors
                    .FirstOrDefaultAsync(d => d.ApplicationUserId == currentUserId, cancellationToken) 
                    ?? throw new UnauthorizedAccessException("Doctor profile not found.");
                
                if (doctor.Id != appointment.DoctorId)
                    throw new UnauthorizedAccessException("You can only cancel appointments from your own schedule.");

                return;
            }

            throw new UnauthorizedAccessException("Your role does not allow cancelling appointments.");
        }
    }
}
