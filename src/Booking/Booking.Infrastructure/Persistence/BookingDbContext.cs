using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public class BookingDbContext(DbContextOptions<BookingDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options), IBookingDbContext
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<AppointmentAttachment> AppointmentAttachments { get; set; }
    public DbSet<DoctorScheduleConfig> DoctorScheduleConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(builder =>
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Status)
                .HasConversion<string>();

            builder.Property(a => a.DoctorId).IsRequired();
            builder.Property(a => a.CustomerId).IsRequired();
        });

        modelBuilder.Entity<DoctorScheduleConfig>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property<List<int>>("_workingDays") 
                  .HasColumnName("WorkingDays")        
                  .HasColumnType("integer[]");        
        });

        base.OnModelCreating(modelBuilder);
    }
}