using Booking.Application.Specialties.Dtos;
using Booking.Application.Specialties.Queries;
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

        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<List<SpecialtyDto>>> GetList()
        {
            return await _mediator.Send(new GetSpecialtiesQuery());
        }
    }
}
