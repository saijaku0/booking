using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Extension;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Appointments.Commands.RescheduleAppointment
{
    public class RescheduleAppointmentCommandHandler(
        IBookingDbContext context,
        IIdentityService identityService,
        ICurrentUserService userService)
        : IRequestHandler<RescheduleAppointmentCommand, Unit>
    {
        private readonly IBookingDbContext _context = context;
        private readonly IIdentityService _identityService = identityService;
        private readonly ICurrentUserService _currentUserService = userService;

        public async Task<Unit> Handle(
            RescheduleAppointmentCommand request, 
            CancellationToken token)
        {
            var userId = _currentUserService.UserId;

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, token)
                ?? throw new KeyNotFoundException($"Appointment {request.AppointmentId} not found.");

            var isAdmin = await _identityService.IsInRoleAsync(userId, Roles.Admin);
            var isDoctor = appointment.Doctor?.ApplicationUserId == userId;
            if (!isAdmin && !isDoctor)
                throw new ForbiddenAccessException("You are not allowed to confirm this appointment.");

            var isSlotTaken = await _context.Appointments
                .Where(a => a.Id != appointment.Id) 
                .Where(a => a.Status != AppointmentStatus.Canceled) 
                .WhereOverlaps(appointment.DoctorId, 
                    request.NewStartTime, 
                    request.NewEndTime)
                .AnyAsync(token);

            if (isSlotTaken)
                throw new InvalidOperationException("The new time slot is already occupied.");

            appointment.Reschedule(request.NewStartTime, request.NewEndTime);

            await _context.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
