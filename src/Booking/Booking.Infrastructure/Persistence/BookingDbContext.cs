using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(builder =>
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Status)
                .HasConversion<string>();

            builder.Property(a => a.ResourceId).IsRequired();
            builder.Property(a => a.CustomerId).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}