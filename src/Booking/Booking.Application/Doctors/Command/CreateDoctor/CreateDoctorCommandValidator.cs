using Booking.Application.Doctors.Command.CreateDoctor;
using FluentValidation;

namespace Booking.Application.Admin.Commands.CreateDoctor
{
    internal class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
    {
        public CreateDoctorCommandValidator() 
        {
            RuleFor(v => v.Email).NotEmpty().EmailAddress();
            RuleFor(v => v.Password).NotEmpty().MinimumLength(6);
            RuleFor(v => v.Name).NotEmpty();
        }
    }
}
