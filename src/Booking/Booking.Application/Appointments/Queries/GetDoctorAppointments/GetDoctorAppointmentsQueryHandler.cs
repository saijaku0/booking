using Booking.Application.Appointments.Dtos;
using Booking.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Appointments.Queries.GetDoctorAppointments;

public class GetDoctorAppointmentsQueryHandler(
    IBookingDbContext bookingDbContext,
    ICurrentUserService currentUserService,
    IIdentityService identityService) 
    : IRequestHandler<GetDoctorAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IBookingDbContext _context = bookingDbContext;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IIdentityService _identityService = identityService;

    public async Task<List<AppointmentDto>> Handle(
        GetDoctorAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var doctorId = await GetCurrentDoctorIdAsync(cancellationToken);

        var query = _context.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId);

        if (request.Start.HasValue)
            query = query.Where(a => a.StartTime >= request.Start.Value);

        if (request.End.HasValue)
            query = query.Where(a => a.EndTime <= request.End.Value);

        var appointments = await query
            .OrderBy(a => a.StartTime)
            .ToListAsync(cancellationToken);

        var resultDtos = new List<AppointmentDto>();

        foreach (var app in appointments)
        {
            var patientName = await _identityService.GetUserNameAsync(app.CustomerId.ToString())
                              ?? "Unknown Patient";

            resultDtos.Add(new AppointmentDto
            {
                Id = app.Id,

                DoctorId = app.DoctorId,

                CustomerId = app.CustomerId,
                PatientName = patientName, 

                StartTime = app.StartTime,
                EndTime = app.EndTime,
                Status = app.Status.ToString(),
                MedicalNotes = app.MedicalNotes
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
            .Where(d => d.UserId == currentUserId)
            .Select(d => d.Id)
            .FirstOrDefaultAsync(token);

        if (doctorId == Guid.Empty)
            throw new UnauthorizedAccessException("User is not a registered doctor profile.");

        return doctorId;
    }
}