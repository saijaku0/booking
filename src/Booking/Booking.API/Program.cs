using Booking.API.Infrastructure;
using Booking.Application.Appointments.Commands.CreateAppointment;
using Booking.Application.Common.Behaviors;
using Booking.Domain.Entities;
using Booking.Infrastructure;
using Booking.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<BookingDbContext>()
    .AddDefaultTokenProviders();

builder.Services.GetServiceCollection(builder.Configuration);

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
    app.MapOpenApi();
    app.MapScalarApiReference();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<Booking.API.Middleware.ApiKeyMiddleware>();

app.MapControllers();
app.Run();