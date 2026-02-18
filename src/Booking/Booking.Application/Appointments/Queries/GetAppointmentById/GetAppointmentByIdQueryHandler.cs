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
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Patient) 
                    .ThenInclude(p => p.ApplicationUser)
                .Include(a => a.Attachments)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), request.Id);

            var doctorUser = appointment.Doctor?.ApplicationUser;
            var patientUser = appointment.Patient?.ApplicationUser;

            var doctorName = doctorUser != null ? $"{doctorUser.FirstName} {doctorUser.LastName}" : "Unknown Doctor";
            var patientName = patientUser != null ? $"{patientUser.FirstName} {patientUser.LastName}" : "Unknown Patient";

            return new AppointmentDetailDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString(),
                MedicalNotes = appointment.MedicalNotes,

                DoctorName = doctorName,
                DoctorPhotoUrl = doctorUser?.PhotoUrl,
                DoctorPhoneNumber = doctorUser?.PhoneNumber,
                Price = appointment.Doctor?.ConsultationFee ?? 0,
                Specialty = appointment.Doctor?.Specialty?.Name ?? "General",

                Attachments = appointment.Attachments.ToAttachmentDtos(),
                PatientName = patientName
            };
        }
    }
}
