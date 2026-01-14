using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using DoctorEntity = Booking.Domain.Entities.Doctor;

namespace Booking.Application.Common.Interfaces
{
    public interface IBookingDbContext
    {
        DbSet<Appointment> Appointments { get; }
        DbSet<DoctorEntity> Doctors { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
