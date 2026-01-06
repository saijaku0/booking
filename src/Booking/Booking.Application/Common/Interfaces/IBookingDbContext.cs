using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Common.Interfaces
{
    public interface IBookingDbContext
    {
        DbSet<Appointment> Appointments { get; }
        DbSet<Doctor> Doctors { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
