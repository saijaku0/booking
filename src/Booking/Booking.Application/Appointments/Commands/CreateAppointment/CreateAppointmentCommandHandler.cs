using Booking.Application.Common.Extension;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentCommandHandler(IBookingDbContext context, ICurrentUserService currentUserService) 
        : IRequestHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IBookingDbContext _context = context;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var isOverLap = await _context.Appointments
                .WhereOverlaps(request.ResourceId,
                    request.StartTime,
                    request.EndTime)
                .AnyAsync(cancellationToken);

            if (isOverLap)
            {
                var failure = new FluentValidation.Results.ValidationFailure("TimeSlot", "This time slot is already taken");
                throw new ValidationException([failure]);
            }

            var userId = Guid.Parse(_currentUserService.UserId);

            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException("User ID is invalid or missing.");

            Appointment appointment = new(
                userId,
                request.ResourceId,
                request.StartTime,
                request.EndTime
            );

            _context.Appointments.Add( appointment );

            await _context.SaveChangesAsync(cancellationToken);

            return appointment.Id;
        }
    }
}
