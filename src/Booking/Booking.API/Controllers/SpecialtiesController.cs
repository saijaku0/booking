using Booking.Application.Admin.Commands.CreateSpecialty;
using Booking.Application.Specialties.Dtos;
using Booking.Application.Specialties.Queries;
using Booking.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SpecialtiesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Creates a new medical specialty.
        /// </summary>
        /// <remarks>
        /// **Requires Admin role.**
        /// <br />
        /// Adds a new entry to the list of available medical specialties (e.g., 'Cardiology', 'Neurology').
        /// </remarks>
        /// <param name="createSpecialty">The command containing the name of the new specialty.</param>
        /// <returns>The GUID of the created specialty.</returns>
        /// <response code="200">Success. Returns the new Specialty ID.</response>
        /// <response code="400">If the specialty name is empty or already exists.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the 'Admin' role.</response>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Guid>> CreateSpecialty(
            CreateSpecialtyCommand createSpecialty)
        {
            return await _mediator.Send(createSpecialty);
        }

        /// <summary>
        /// Retrieves a list of all available medical specialties.
        /// </summary>
        /// <remarks>
        /// Public endpoint. Use this to populate dropdowns in the UI.
        /// </remarks>
        /// <returns>List of specialties (ID and Name)</returns>
        /// <response code="200">Returns the list (can be empty)</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<SpecialtyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SpecialtyDto>>> GetList()
        {
            return await _mediator.Send(new GetSpecialtiesQuery());
        }

        /// <summary>
        /// Retrieves details of a specific specialty by ID.
        /// </summary>
        /// <param name="id">Unique identifier of the specialty</param>
        /// <response code="200">Returns the specialty details</response>
        /// <response code="404">Specialty not found</response>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SpecialtyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SpecialtyDto>> GetById(Guid id)
        {
            return await _mediator.Send(new GetSpecialtyByIdQuery(id));
        }
    }
}
