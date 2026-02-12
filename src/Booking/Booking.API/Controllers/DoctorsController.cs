using Booking.API.Dtos.Doctor;
using Booking.Application.Doctors.Command.CreateDoctor;
using Booking.Application.Doctors.Command.UpdateDoctor;
using Booking.Application.Doctors.Command.UpdateProfilePhoto;
using Booking.Application.Doctors.Command.UpdateScheduleConfig;
using Booking.Application.Doctors.Dtos;
using Booking.Application.Doctors.Queries.GetDoctorById;
using Booking.Application.Doctors.Queries.GetDoctors;
using Booking.Application.Doctors.Queries.GetDoctorStats;
using Booking.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DoctorsController(IMediator mediator)
        : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Registers a new doctor in the system.
        /// </summary>
        /// <remarks>
        /// **Requires Admin role.**
        /// <br />
        /// This endpoint creates a new user account with the 'Doctor' role and a corresponding doctor profile linked to a specialty.
        /// </remarks>
        /// <param name="createDoctor">The details for the new doctor (First Name, Last Name, Email, SpecialtyId).</param>
        /// <returns>A JSON object containing the ID of the newly created doctor.</returns>
        /// <response code="200">Success. Returns the new Doctor ID.</response>
        /// <response code="400">Validation failed (e.g., missing required fields or invalid email).</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the 'Admin' role.</response>
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateNewDoctor(
            [FromBody] CreateDoctorCommand createDoctor)
        {
            var id = await _mediator.Send(createDoctor);

            return Ok(new { id });
        }

        /// <summary>
        /// Get a list of all doctors with optional filtering
        /// </summary>
        /// <param name="getDoctors">Filter parameters (search term, specialty)</param>
        /// <returns>List of doctors</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<DoctorDto>>> GetListDoctors(
            [FromQuery] GetDoctorsQuery getDoctors)
        {
            var doctorsList = await _mediator.Send(getDoctors);

            return Ok(doctorsList);
        }

        /// <summary>
        /// Updates the profile of an existing doctor.
        /// </summary>
        /// <remarks>
        /// Only the doctor themselves or an administrator can perform this action.
        /// The ID in the URL must match the ID in the body.
        /// </remarks>
        /// <param name="id">The unique identifier of the doctor (GUID).</param>
        /// <param name="updateDoctor">The object containing updated profile data.</param>
        /// <returns>No Content if successful.</returns>
        /// <response code="204">Profile updated successfully.</response>
        /// <response code="400">If the ID in URL and Body do not match.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user tries to edit someone else's profile.</response>
        /// <response code="404">If the doctor was not found.</response>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateDoctorProfile(
            Guid id,
            [FromBody] UpdateDoctorCommand updateDoctor)
        {
            if (updateDoctor.UserId != Guid.Empty && updateDoctor.UserId != id)
                return BadRequest("Mismatched doctor ID");

            await _mediator.Send(updateDoctor);

            return NoContent();
        }

        /// <summary>
        /// Uploads or updates the doctor's profile photo.
        /// </summary>
        /// <remarks>
        /// Accepts an image file (jpg, png). 
        /// Max file size is usually limited by server settings (default ~30MB).
        /// </remarks>
        /// <param name="file">The image file to upload</param>
        /// <response code="204">Photo updated successfully</response>
        /// <response code="400">File is empty</response>
        /// <response code="401">User is not authorized</response>
        [HttpPost("profile-photo")]
        [Authorize(Roles = Roles.Doctor)]
        public async Task<IActionResult> UpdatePhoto(
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            using var stream = file.OpenReadStream();

            var command = new UpdateDoctorPhotoCommand(
                stream,
                file.FileName,
                file.ContentType
            );

            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Retrieves full details of a specific doctor (including rating).
        /// </summary>
        /// <param name="id">Doctor Id</param>
        /// <returns>Doctor information</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDto>> GetDoctorById(Guid id)
        {
            var doctor = await _mediator.Send(new GetDoctorByIdQuery(id));
            return Ok(doctor);
        }

        /// <summary>
        /// Retrieves statistics for a specific doctor over a given period.
        /// </summary>
        /// <remarks> 
        /// This endpoint allows you to retrieve statistics for a specific doctor over a specified period.
        /// The period can be "day", "week", or "month".
        /// </remarks>
        /// <param name="id">Doctor ID</param>
        /// <param name="period">The period for which to retrieve statistics (day, week, month)</param>
        /// <returns>Doctor statistics</returns>
        /// <response code="200">Returns the doctor statistics</response>
        /// <response code="404">If the doctor was not found</response>
        [HttpGet("{id:guid}/stats")]
        [Authorize(Roles = Roles.Doctor + "," + Roles.Admin)]
        [ProducesResponseType(typeof(DoctorStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorStatsDto>> GetDoctorStats(
            Guid id, 
            [FromQuery] string period)
        {
            var stats = await _mediator.Send(new GetDoctorStatsQuery(id, period));
            return Ok(stats);
        }

        /// <summary>
        /// Updates the schedule configuration for the specified doctor.    
        /// </summary>
        /// <remarks>Only users with the Doctor or Admin role are authorized to perform this
        /// operation.</remarks>
        /// <param name="id">The unique identifier of the doctor whose schedule is being updated.</param>
        /// <param name="request">An object containing the new schedule configuration details. Cannot be null.</param>
        /// <returns>A response indicating the result of the update operation. Returns status code 204 (No Content) if the update
        /// is successful.</returns>
        /// <response code="204">Schedule updated successfully.</response>
        [HttpPut("{id:guid}/schedule")]
        [Authorize(Roles = Roles.Doctor + "," + Roles.Admin)] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateSchedule(
            Guid id, 
            [FromBody] UpdateScheduleRequest request)
        {
            var command = new UpdateScheduleConfigCommand(
                id,
                request.DayStart,
                request.DayEnd,
                request.LunchStart,
                request.LunchEnd,
                request.WorkingDays,
                request.SlotDurationMinutes,
                request.BufferMinutes
            );

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
