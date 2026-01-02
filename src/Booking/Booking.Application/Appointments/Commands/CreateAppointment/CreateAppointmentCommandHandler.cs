using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentCommandHandler(IBookingDbContext context) : IRequestHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var isOverLap = await _context.Appointments
                .AnyAsync(x =>
                    x.ResourceId == request.ResourceId
                        &&
                    request.StartTime < x.EndTime
                        &&
                    request.EndTime > x.StartTime,
                    cancellationToken);

            if (isOverLap)
            {
                var failure = new FluentValidation.Results.ValidationFailure("TimeSlot", "This time slot is already taken");
                throw new ValidationException([failure]);
            }

            Appointment appointment = new(
                request.CustomerId,
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
