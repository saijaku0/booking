using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Commands.ConfirmAppointment
{
    public class ConfirmAppointmentCommandHandler(
        IBookingDbContext dbContext) : IRequestHandler<ConfirmAppointmentCommand, Unit>
    {
        private readonly IBookingDbContext _dbContext = dbContext;

        public async Task<Unit> Handle(
            ConfirmAppointmentCommand request, 
            CancellationToken cancellationToken)
        {
            var appointment = await _dbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken)
                ?? throw new KeyNotFoundException("Appointment not found.");

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
