using Booking.Application.Appointments.Common.Extensions;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Exceptions;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetAppointmentById
{
    public class GetAppointmentByIdQueryHandler(IBookingDbContext context) 
        : IRequestHandler<GetAppointmentByIdQuery, AppointmentDetailDto>
    {
        private readonly IBookingDbContext _context = context;

        public async Task<AppointmentDetailDto> Handle(
            GetAppointmentByIdQuery request, 
            CancellationToken cancellationToken)
        {
            var appointment = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Attachments)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), request.Id);

            var DoctorName = $"{appointment.Doctor.ApplicationUser.FirstName} {appointment.Doctor.ApplicationUser.LastName}";
            var PatientName = $"{appointment.Patient.ApplicationUser.FirstName} {appointment.Patient.ApplicationUser.LastName}";

            return new AppointmentDetailDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,

                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,

                Status = appointment.Status.ToString(),
                MedicalNotes = appointment.MedicalNotes,

                DoctorName = DoctorName,
                DoctorPhotoUrl = appointment.Doctor.ApplicationUser.PhotoUrl,
                DoctorPhoneNumber = appointment.Doctor.ApplicationUser.PhoneNumber,
                Price = appointment.Doctor.ConsultationFee,
                Specialty = appointment.Doctor.Specialty.Name,

                Attachments = appointment.Attachments.ToAttachmentDtos(),

                PatientName = PatientName
            };
        }
    }
}
