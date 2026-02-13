using Booking.Application.Patients.Dtos;
using Booking.Application.Patients.Queries.GetPatientProfile;
using Booking.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PatientController(IMediator mediatR) : ControllerBase
    {
        private readonly IMediator _mediator = mediatR;

        /// <summary>
        /// Gets the profile of the currently authenticated patient.
        /// </summary>
        /// <remarks>Requires the user to be authenticated as a patient.</remarks>
        /// <returns>The patient's profile information.</returns>
        /// <response code="200">Returns the patient's profile.</response>
        /// <response code="401">Unauthorized - the user is not authenticated or does not have the patient role.</response>
        /// <response code="404">Not Found - the patient's profile could not be found.</response>
        /// <response code="500">Internal Server Error - an unexpected error occurred while processing the request.</response>
        [HttpGet("profile")]
        [Authorize(Roles = Roles.Patient)]
        [ProducesResponseType(typeof(PatientProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PatientProfileDto>> GetMyProfile()
        {
            var query = new GetPatientProfileQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
