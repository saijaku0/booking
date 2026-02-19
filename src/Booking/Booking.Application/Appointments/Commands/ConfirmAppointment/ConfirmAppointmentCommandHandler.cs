using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.ConfirmAppointment
{
    public class ConfirmAppointmentCommandHandler(
        IBookingDbContext dbContext,
        ICurrentUserService userService,
        IIdentityService identityService) : IRequestHandler<ConfirmAppointmentCommand, Unit>
    {
        private readonly IBookingDbContext _dbContext = dbContext;
        private readonly ICurrentUserService _userService = userService;
        private readonly IIdentityService _identityService = identityService;

        public async Task<Unit> Handle(
            ConfirmAppointmentCommand request, 
            CancellationToken cancellationToken)
        {
            var userId = _userService.UserId 
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken)
                ?? throw new KeyNotFoundException("Appointment not found.");

            var isAdmin = await _identityService.IsInRoleAsync(userId, Roles.Admin);
            var isDoctor = appointment.Doctor?.ApplicationUserId == userId;
            if (!isAdmin && !isDoctor)
                throw new ForbiddenAccessException("You are not allowed to confirm this appointment.");

            if (appointment.Status == AppointmentStatus.Confirmed)
                return Unit.Value;

            if (appointment.Status == AppointmentStatus.Canceled)
                throw new InvalidOperationException("Cannot confirm a canceled appointment.");

            appointment.Confirm();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
