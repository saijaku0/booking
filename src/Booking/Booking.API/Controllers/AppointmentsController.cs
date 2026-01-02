using Booking.Application.Appointments.Commands.CreateAppointment;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Appointments.Queries.GetAppointmentById;
using Booking.Application.Appointments.Queries.GetAppointmentsByDate;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppointmentsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentCommand command)
        {
            var id = await _mediator.Send(command);

            return CreatedAtAction(
                   nameof(GetById),
                   new { id },
                   id
               );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var appointment = await _mediator.Send(new GetAppointmentByIdQuery { Id = id });

            return Ok(appointment); 
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentDto>>> GetAppointmentsByDateQuery(
            [FromQuery] Guid resourceId,
            [FromQuery] DateTime start, 
            [FromQuery] DateTime end)
        {
            var getAppointmentsDate = await _mediator.Send(new GetAppointmentsByDateQuery
            {
                ResourceId = resourceId,
                StartTime = start,
                EndTime = end
            });

            return Ok(getAppointmentsDate);
        }
    }
}
