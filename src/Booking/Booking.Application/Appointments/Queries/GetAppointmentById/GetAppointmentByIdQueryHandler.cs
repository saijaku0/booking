using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetAppointmentById
{
    public class GetAppointmentByIdQueryHandler(IBookingDbContext context) 
        : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<AppointmentDto> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,

                    StartTime = a.StartTime,
                    EndTime = a.EndTime,

                    Status = a.Status.ToString(),
                    MedicalNotes = a.MedicalNotes,

                    DoctorName = $"{a.Doctor.ApplicationUser.FirstName} {a.Doctor.ApplicationUser.LastName}",
                    DoctorPhotoUrl = a.Doctor.ApplicationUser.PhotoUrl,
                    DoctorPhoneNumber = a.Doctor.ApplicationUser.PhoneNumber,
                    Price = a.Doctor.ConsultationFee,
                    Specialty = a.Doctor.Specialty.Name,

                    PatientName = $"{a.Patient.ApplicationUser.FirstName} {a.Patient.ApplicationUser.LastName}"
                })
                .FirstOrDefaultAsync(cancellationToken);

                if (appointment == null)
                    throw new NotFoundException(nameof(appointment), request.Id);

            return appointment;
        }
    }
}
