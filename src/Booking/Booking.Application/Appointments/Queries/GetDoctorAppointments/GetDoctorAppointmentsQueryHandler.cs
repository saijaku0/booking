using Booking.Application.Appointments.Common.Extensions;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace Booking.Application.Appointments.Queries.GetDoctorAppointments;

public class GetDoctorAppointmentsQueryHandler(
    IBookingDbContext bookingDbContext,
    ICurrentUserService currentUserService) 
    : IRequestHandler<GetDoctorAppointmentsQuery, List<AppointmentDetailDto>>
{
    private readonly IBookingDbContext _context = bookingDbContext;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<List<AppointmentDetailDto>> Handle(
        GetDoctorAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var doctorId = await GetCurrentDoctorIdAsync(cancellationToken);

        var query = _context.Appointments
            .AsNoTracking()
            .Include(a => a.Attachments)
            .Include(a => a.Patient)
                .ThenInclude(p => p.ApplicationUser)
            .Where(a => a.DoctorId == doctorId);

        if (request.Start.HasValue)
            query = query.Where(a => a.StartTime >= request.Start.Value);

        if (request.End.HasValue)
            query = query.Where(a => a.EndTime <= request.End.Value);

        var appointments = await query
            .OrderBy(a => a.StartTime)
            .ToListAsync(cancellationToken);

        var resultDtos = new List<AppointmentDetailDto>();

        foreach (var app in appointments)
        {
            var firstName = app.Patient?.ApplicationUser?.FirstName ?? "Unknown";
            var lastName = app.Patient?.ApplicationUser?.LastName ?? "Patient";
            var patientName = $"{firstName} {lastName}";

            resultDtos.Add(new AppointmentDetailDto
            {
                Id = app.Id,

                DoctorId = app.DoctorId,

                PatientId = app.PatientId,
                PatientName = patientName,

                StartTime = app.StartTime,
                EndTime = app.EndTime,
                Status = app.Status.ToString(),
                MedicalNotes = app.MedicalNotes ?? string.Empty,

                Attachments = app.Attachments.ToAttachmentDtos(),
            });
        }

        return resultDtos;
    }

    private async Task<Guid> GetCurrentDoctorIdAsync(CancellationToken token)
    {
        var currentUserId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(currentUserId))
            throw new UnauthorizedAccessException("User is not logged in.");

        var doctorId = await _context.Doctors
            .Where(d => d.ApplicationUserId == currentUserId)
            .Select(d => d.Id)
            .FirstOrDefaultAsync(token);

        if (doctorId == Guid.Empty)
            throw new UnauthorizedAccessException("User is not a registered doctor profile.");

        return doctorId;
    }
}