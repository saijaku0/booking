using Booking.API.Infrastructure;
using Booking.Application.Appointments.Commands.CreateAppointment;
using Booking.Application.Common.Behaviors;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using Booking.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<BookingDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<BookingDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IBookingDbContext>(provider =>
    (IBookingDbContext)provider.GetRequiredService<BookingDbContext>());

builder.Services.AddValidatorsFromAssembly(typeof(CreateAppointmentCommand).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentCommand).Assembly);

    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
}); 

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.MapControllers();
app.Run();