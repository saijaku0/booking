using Booking.Application.Appointments.Commands.CancelAppointment;
using Booking.Application.Appointments.Commands.CompleteAppointment;
using Booking.Application.Appointments.Commands.CreateAppointment;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Appointments.Queries.GetAppointmentById;
using Booking.Application.Appointments.Queries.GetAppointmentsByDate;
using Booking.Application.Appointments.Queries.GetDoctorAppointments;
using Booking.Application.Appointments.Queries.GetDoctorAvailability;
using Booking.Application.Appointments.Queries.GetPatientAppointments;
using Booking.Domain.Constants;
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
        /// Allows you to filter reservations by a specific room (doctorId) or get a general schedule.
        /// </remarks>
        /// <param name="doctorId">ID (can be null)</param>
        /// <param name="start">sart of a period</param>
        /// <param name="end">end of a period</param>
        /// <returns>List of booking</returns>
        /// <response code="200">Successful request. Returns a list (may be empty)</response>
        [HttpGet]
        public async Task<ActionResult<List<AppointmentDto>>> GetAppointmentsByDateQuery(
            [FromQuery] Guid doctorId,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var getAppointmentsDate = await _mediator.Send(new GetAppointmentsByDateQuery
            {
                DoctorId = doctorId,
                StartTime = start,
                EndTime = end
            });

            return Ok(getAppointmentsDate);
        }

        /// <summary>
        /// Retrieves the personal schedule for the currently logged-in doctor.
        /// </summary>
        /// <remarks>
        /// This endpoint is restricted to users with the 'doctor' role.
        /// It automatically identifies the doctor based on the authentication token.
        /// If date parameters are omitted, returns all appointments.
        /// </remarks>
        /// <param name="start">Optional start date filter (UTC). If null, includes appointments from the beginning of time.</param>
        /// <param name="end">Optional end date filter (UTC). If null, includes appointments up to the end of time.</param>
        /// <returns>A list of appointments belonging to the current doctor</returns>
        /// <response code="200">Returns the list of appointments (can be empty).</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="403">User is authorized but does not have the 'doctor' role.</response>
        [Authorize(Roles = Roles.Doctor)]
        [HttpGet("me")]
        public async Task<ActionResult<List<AppointmentDto>>> GetDoctorAppointmentsQuery(
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end)
        {
            var getAppointments = await _mediator.Send(new GetDoctorAppointmentsQuery
            {
                Start = start,
                End = end
            });

            return Ok(getAppointments);
        }

        /// <summary>
        /// Cancels an existing appointment.
        /// </summary>
        /// <remarks>
        /// - Patients can cancel only their own appointments.
        /// - Doctors can cancel appointments only in their schedule.
        /// - Admins can cancel any appointment.
        /// </remarks>
        /// <param name="id">Appointment ID</param>
        /// <response code="204">Success (No Content)</response>
        /// <response code="403">Forbidden (Trying to cancel someone else's booking)</response>
        /// <response code="404">Appointment not found</response>
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> CancelAppointment(Guid id)
        {
            await _mediator.Send(new CancelAppointmentCommand(id));

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doctorId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("availability")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TimeSlotDto>>> GetAvailability(
            [FromQuery] Guid doctorId,
            [FromQuery] DateTime date)
        {
            return await _mediator.Send(new GetDoctorAvailabilityQuery(doctorId, date));
        }

        /// <summary>
        /// Marks an appointment as completed.
        /// </summary>
        /// <remarks>
        /// Can only be performed by the assigned doctor or an admin.
        /// The appointment must be in 'Confirmed' status.
        /// </remarks>
        /// <param name="id">Appointment ID</param>
        /// <param name="notes">Doctor medical notes</param>
        /// <response code="204">Success</response>
        /// <response code="400">Invalid status (e.g. already canceled)</response>
        /// <response code="403">Not authorized to complete this appointment</response>
        [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> CompleteAppointment(
            Guid id,
            [FromBody] string notes)
        {
            await _mediator.Send(new CompleteAppointmentCommand(id, notes));
            return NoContent();
        }

        /// <summary>
        /// Retrieves the personal schedule for the currently logged-in patient.
        /// </summary>
        /// <remarks>
        /// Returns a list of all appointments (past and future) where the current user is the patient.
        /// The list is ordered by StartTime descending (newest first).
        /// </remarks>
        /// <returns>A list of appointments</returns>
        /// <response code="200">Returns the list of appointments (can be empty)</response>
        /// <response code="401">User is not authorized (token missing or invalid)</response>
        /// <response code="403">User is authorized but is not a Patient (e.g. a Doctor trying to view patient records)</response>
        [Authorize(Roles = Roles.Patient)]
        [HttpGet("my")]
        [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<AppointmentDto>>> GetPatientAppointments()
        {
            return await _mediator.Send(new GetPatientAppointmentsQuery());
        }
    }
}
