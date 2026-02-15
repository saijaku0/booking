using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Doctors.Command.DeleteDoctor
{
    public class DeleteDoctorCommandHandler(
        IBookingDbContext context,
        IIdentityService identityService)
        : IRequestHandler<DeleteDoctorCommand>
    {
        private readonly IBookingDbContext _context = context;
        private readonly IIdentityService _identityService = identityService;

        public async Task Handle(
            DeleteDoctorCommand request, 
            CancellationToken cancellationToken)
        {
            var doctor = await _context.Doctors
            .Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Doctor), request.DoctorId);
            

            var hasActiveAppointments = doctor.Appointments
                .Any(a => a.StartTime > DateTime.UtcNow && a.Status != AppointmentStatus.Canceled);

            if (hasActiveAppointments)
                throw new Exception("Cannot delete doctor with active future appointments. Please cancel or reschedule them first.");

            var identityResult = await _identityService.DisableUserAsync(doctor.ApplicationUserId);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors);
                throw new Exception($"Failed to disable user account: {errors}");
            }

            doctor.Deactivate();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
