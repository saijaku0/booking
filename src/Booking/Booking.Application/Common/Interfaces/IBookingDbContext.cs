using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.Common.Interfaces
{
    public interface IBookingDbContext
    {
        DbSet<Appointment> Appointments { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
