using Booking.API.Dtos.Appointment;
using Booking.Application.Appointments.Commands.CancelAppointment;
using Booking.Application.Appointments.Commands.CompleteAppointment;
using Booking.Application.Appointments.Commands.ConfirmAppointment;
using Booking.Application.Appointments.Commands.CreateAppointment;
using Booking.Application.Appointments.Commands.DeleteAttachment;
using Booking.Application.Appointments.Commands.RescheduleAppointment;
using Booking.Application.Appointments.Commands.UploadAttachment;
using Booking.Application.Appointments.Dtos;
using Booking.Application.Appointments.Queries.GetAppointmentById;
using Booking.Application.Appointments.Queries.GetAppointmentReport;
using Booking.Application.Appointments.Queries.GetAppointmentsByDate;
using Booking.Application.Appointments.Queries.GetDoctorAppointments;
using Booking.Application.Appointments.Queries.GetDoctorAvailability;
using Booking.Application.Appointments.Queries.GetPatientAppointments;
using Booking.Domain.Constants;
using Booking.Domain.Entities;
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
        /// <param name="command">Command containing the appointment details to create (patient, doctor, time range and any required metadata).</param>
        /// <returns>The identifier of the newly created appointment.</returns>
        /// <response code="200">Successfully created. The Location header will contain a link to the resource</response>
        /// <summary>
        /// Creates a new appointment for the authenticated patient.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(typeof(AppointmentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
        [ProducesResponseType(typeof(List<AppointmentDetailDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AppointmentDetailDto>>> GetAppointmentsByDateQuery(
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
        [HttpGet("doctor-schedule")]
        [Authorize(Roles = Roles.Doctor)]
        [ProducesResponseType(typeof(List<AppointmentDetailDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AppointmentDetailDto>>> GetDoctorAppointmentsQuery(
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
        [Authorize]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [AllowAnonymous]
        [HttpGet("availability")]
        [ProducesResponseType(typeof(List<TimeSlotDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TimeSlotDto>>> GetAvailability(
            [FromQuery] Guid doctorId,
            [FromQuery] DateTime date)
        {
            return await _mediator.Send(new GetDoctorAvailabilityQuery(doctorId, date));
        }

        /// <summary>
        /// Marks the specified appointment as complete.
        /// </summary>
        /// <remarks>
        /// Can only be performed by the assigned doctor or an admin.
        /// The appointment must be in 'Confirmed' status.
        /// </remarks>
        /// <param name="id">The unique identifier of the appointment to complete. Must match command.AppointmentId.</param>
        /// <param name="command">Data required to complete the appointment, including diagnosis and optional medical notes.</param>
        /// <returns>
        /// 204 No Content on success; 400 Bad Request if <paramref name="id"/> does not match <c>command.AppointmentId</c>; 403 if the caller is not authorized.
        /// </returns>
        /// <response code="403">Not authorized to complete this appointment</response>
        [HttpPost("{id:guid}/complete")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CompleteAppointment(
            Guid id,
            [FromBody] CompleteAppointmentCommand command)
        {
            if (id != command.AppointmentId) return BadRequest();

            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Confirms an existing appointment.
        /// </summary>
        /// <remarks>
        /// Can only be performed by the assigned doctor or an admin.
        /// The appointment must be in 'Pending' status.
        /// </remarks>
        /// <param name="id">Appointment ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Success</response>
        /// <response code="404">Appointment not found</response>
        [HttpPost("{id}/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmAppointment(Guid id)
        {
            await _mediator.Send(new ConfirmAppointmentCommand(id));

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
        [HttpGet("patient-history")]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(typeof(List<AppointmentListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<AppointmentListDto>>> GetPatientAppointments()
        {
            return await _mediator.Send(new GetPatientAppointmentsQuery());
        }

        /// <summary>
        /// Downloads a PDF medical report for a specific appointment.
        /// </summary>
        /// <remarks>
        /// Returns a PDF file containing the medical report for the specified appointment. 
        /// The report includes details such as appointment date, doctor information, patient information, 
        /// and any notes or findings recorded by the doctor. This endpoint is typically used by patients to access their medical reports after an appointment 
        /// has been completed. Access to the report is restricted to the patient associated with the appointment and authorized medical staff.
        /// </remarks>
        /// <returns>A PDF file containing the medical report</returns>
        /// <response code="200">Returns the PDF medical report</response>
        [Authorize]
        [HttpGet("{id:guid}/report")]
        [Produces("application/pdf")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReport(Guid id)
        {
            var query = new GetAppointmentReportQuery(id);
            var result = await _mediator.Send(query);

            return File(result.Content, result.ContentType, result.FileName);
        }

        /// <summary>
        /// Reschedules an existing appointment with new start and end times.   
        /// </summary>
        /// <remarks>This action is restricted to users with the Doctor role. The appointment must exist
        /// and be eligible for rescheduling.</remarks>
        /// <param name="id">The appointment's unique identifier.</param>
        /// <param name="request">The new start and end times for the appointment.</param>
        /// <response code="204">The appointment was successfully rescheduled.</response>
        /// <response code="400">The request is invalid (e.g., inconsistent or missing times).</response>
        /// <response code="404">No appointment was found with the specified ID.</response>
        /// <returns>HTTP 204 No Content on success.</returns>
        [HttpPut("{id}/reschedule")]
        [Authorize(Roles = Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reschedule(
            Guid id,
            [FromBody] RescheduleAppointmentRequest request)
        {
            await _mediator.Send(new RescheduleAppointmentCommand(id,
                request.StartTime, request.EndTime));
            return NoContent();
        }

        /// <summary>
        /// Uploads an attachment and associates it with the specified appointment.
        /// </summary>
        /// <param name="id">The appointment identifier to attach the file to.</param>
        /// <param name="file">The file to upload as the attachment.</param>
        /// <param name="type">The attachment type.</param>
        /// <returns>The identifier of the created attachment.</returns>
        /// <response code="201">Attachment created; the response body contains the new attachment's identifier.</response>
        /// <response code="400">The request was invalid.</response>
        /// <response code="403">The caller is not authorized to upload attachments for the appointment.</response>
        /// <response code="404">No appointment was found with the specified ID.</response>
        [Authorize(Roles = Roles.Doctor)]
        [HttpPost("{id:guid}/attachments")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadAttachment(
            Guid id,
            [FromForm] IFormFile file,
            [FromForm] AttachmentType type)
        {
            var command = new UploadAttachmentCommand(
                File: file,
                AppointmentId: id,
                FileType: type
            );
            var attachmentId = await _mediator.Send(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id },
                attachmentId
            );
        }

        /// <summary>
        /// Deletes an attachment from the specified appointment.
        /// </summary>
        /// <param name="appointmentId">The identifier of the appointment containing the attachment.</param>
        /// <param name="attachmentId">The identifier of the attachment to delete.</param>
        /// <returns>204 No Content if the attachment was deleted; 404 Not Found if the appointment or attachment does not exist; 403 Forbidden if the caller is not authorized.</returns>
        [Authorize(Roles = Roles.Doctor)]
        [HttpDelete("{appointmentId:guid}/attachments/{attachmentId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAttachment(
            Guid appointmentId,
            Guid attachmentId)
        {
            var command = new DeleteAttachmentCommand(appointmentId, attachmentId);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}