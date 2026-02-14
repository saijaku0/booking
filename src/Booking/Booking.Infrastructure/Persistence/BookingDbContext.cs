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
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasOne(p => p.ApplicationUser)
                  .WithOne(u => u.PatientProfile) 
                  .HasForeignKey<Patient>(p => p.ApplicationUserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Appointments)
                  .WithOne(a => a.Patient)
                  .HasForeignKey(a => a.PatientId) 
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(p => p.ApplicationUserId).IsRequired();
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.HasOne(d => d.ApplicationUser)
                  .WithOne(u => u.DoctorProfile)
                  .HasForeignKey<Doctor>(d => d.ApplicationUserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.ScheduleConfig)
                  .WithOne(c => c.Doctor)
                  .HasForeignKey<DoctorScheduleConfig>(c => c.DoctorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(d => d.ApplicationUserId).IsRequired();
        });

        modelBuilder.Entity<Appointment>(builder =>
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Status)
                   .HasConversion<string>();

            builder.HasOne(a => a.Doctor)
                   .WithMany()
                   .HasForeignKey(a => a.DoctorId)
                   .IsRequired();

            builder.HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(p => p.PatientId)
                   .IsRequired();
        });

        modelBuilder.Entity<DoctorScheduleConfig>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property<List<int>>("_workingDays") 
                  .HasColumnName("WorkingDays")        
                  .HasColumnType("integer[]");   
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(r => r.ReviewId);

            entity.HasOne(r => r.Doctor)
                  .WithMany(d => d.Reviews)
                  .HasForeignKey(r => r.DoctorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Patient)
                  .WithMany(p => p.Reviews) 
                  .HasForeignKey(r => r.PatientId)
                  .OnDelete(DeleteBehavior.Restrict); 
        });

        modelBuilder.Entity<AppointmentAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Appointment>()
                  .WithMany(a => a.Attachments)
                  .HasForeignKey(e => e.AppointmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}