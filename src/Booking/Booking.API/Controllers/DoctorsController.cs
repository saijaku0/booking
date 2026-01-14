using Booking.Application.Doctors.Command.UpdateDoctor;
using Booking.Application.Doctors.Dtos;
using Booking.Application.Doctors.Queries.GetDoctors;
using MediatR;
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
        /// Get a list of all doctors with optional filtering
        /// </summary>
        /// <param name="getDoctors">Filter parameters (search term, specialty)</param>
        /// <returns>List of doctors</returns>
        [HttpGet]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateDoctorProfile(
            Guid id,
            [FromBody] UpdateDoctorCommand updateDoctor)
        {
            if (id.ToString() != updateDoctor.UserId)
                return BadRequest("Mismatched doctor ID");

            await _mediator.Send(updateDoctor);

            return NoContent();
        }
    }
}
