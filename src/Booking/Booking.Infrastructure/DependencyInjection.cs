using Booking.Application.Appointments.Common.Interfaces;
using Booking.Application.Common.Interfaces;
using Booking.Domain.Entities;
using Booking.Infrastructure.Identity;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Booking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection GetServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<BookingDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IBookingDbContext>(provider => provider.GetRequiredService<BookingDbContext>());
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IFileStorageService, LocalFileStorageService>();

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IPdfGenerator, PdfGenerator>();

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BookingDbContext>();

            services.AddHttpContextAccessor();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!))
                };
            });

            return services;
        }
    }
}
