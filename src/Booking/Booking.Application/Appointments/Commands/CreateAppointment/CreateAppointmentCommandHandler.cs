using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Extension;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
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
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (request.StartTime >= request.EndTime)
            {
                throw new ValidationException(new List<ValidationFailure>
                    {
                        new(nameof(request.StartTime), "Start time must be before end time.")
                    });
            }
            if (request.StartTime < DateTime.UtcNow)
            {
                throw new ValidationException(new List<ValidationFailure>
                    {
                        new(nameof(request.StartTime), "Cannot book appointment in the past.")
                    });
            }

            var doctorExists = await _context.Doctors
            .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);
            if (!doctorExists)
                throw new NotFoundException(nameof(Doctor), request.DoctorId);

            var isOverLap = await _context.Appointments
                .Where(a => a.Status != AppointmentStatus.Canceled)
                .WhereOverlaps(request.DoctorId,
                    request.StartTime,
                    request.EndTime)
                .AnyAsync(cancellationToken);

            if (isOverLap)
            {
                var failure = new ValidationFailure(
                    nameof(request.StartTime),
                    "This time slot is already taken.");
                throw new ValidationException(new List<ValidationFailure> { failure });
            }


            var patientId = await _context.Patients
             .Where(p => p.ApplicationUserId == userId)
             .Select(p => p.Id)
             .FirstOrDefaultAsync(cancellationToken);

            if (patientId == Guid.Empty)
            {
                var failure = new ValidationFailure(
                    nameof(userId),
                    $"Patient profile not found for user {userId}. Please complete your profile registration.");
                throw new ValidationException(new List<ValidationFailure> { failure });
            }


            Appointment appointment = new(
                request.DoctorId,
                patientId,
                request.StartTime,
                request.EndTime
            );

            _context.Appointments.Add( appointment );
            await _context.SaveChangesAsync(cancellationToken);

            return appointment.Id;
        }
    }
}
