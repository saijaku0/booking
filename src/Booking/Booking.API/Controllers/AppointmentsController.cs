using Booking.Application.Appointments.Commands.CreateAppointment;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Appointments.Queries.GetAppointmentById;
using Booking.Application.Appointments.Queries.GetAppointmentsByDate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppointmentsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Creates a new reservation
        /// </summary>
        /// <remarks>Checks if the time slot is free. If the slot is occupied or the data is invalid, returns an error</remarks>
        /// <param name="command">Data for creating a reservation</param>
        /// <returns>ID of the created reservation</returns>
        /// <response code="200">Successfully created. The Location header will contain a link to the resource</response>
        /// <response code="400">Validation error (incorrect dates) or overlap with another booking.</response>
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

        /// <summary>
        /// Get reservation by ID
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>Single reservation with date of start and end</returns>
        /// <response code="200">Successful request. Returns a reservation</response>
        /// <response code="404">Invalid ID. Return a errore code</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var appointment = await _mediator.Send(new GetAppointmentByIdQuery { Id = id });

            return Ok(appointment); 
        }

        /// <summary>
        /// Get reservation list by appointment period
        /// </summary>
        /// <remarks>
        /// Allows you to filter reservations by a specific room (resourceId) or get a general schedule.
        /// </remarks>
        /// <param name="resourceId">ID (can be null)</param>
        /// <param name="start">sart of a period</param>
        /// <param name="end">end of a period</param>
        /// <returns>List of booking</returns>
        /// <response code="200">Successful request. Returns a list (may be empty)</response>
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
