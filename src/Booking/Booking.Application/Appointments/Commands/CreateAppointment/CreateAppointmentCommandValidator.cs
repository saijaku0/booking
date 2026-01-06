using FluentValidation;

namespace Booking.Application.Appointments.Commands.CreateAppointment
{
    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator() 
        {
            RuleFor(v => v.DoctorId)
                .NotEmpty().WithMessage("Enter resorce ID");

            RuleFor(v => v.StartTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Reservations are not possible in the past");

            RuleFor(v => v.EndTime)
                .GreaterThan(v => v.StartTime).WithMessage("The end date must be later than the start date");
        }
    }
}
