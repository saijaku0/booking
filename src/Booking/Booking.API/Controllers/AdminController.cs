using Booking.Application.Admin.Commands.CreateDoctor;
using Booking.Application.Admin.Commands.CreateSpecialty;
using Booking.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Booking.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdminController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("doctors")]
        public async Task<IActionResult> AddNewDoctor(
            [FromBody] CreateDoctorCommand createDoctor)
        {
            var id = await _mediator.Send(createDoctor);

            return Ok(new { id });
        }

        // TO DO: create GetDoctorByIdQuery query method 
        //[Authorize(Roles = Roles.Admin)]
        //[HttpGet("{id:guid}")]
        //public async Task<IActionResult> GetDoctorById(Guid id)
        //{
        //    var appointment = await _mediator.Send(new GetDoctorByIdQuery { Id = id });

        //    return Ok(appointment);
        //}

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("specialties")]
        public async Task<ActionResult<Guid>> CreateSpecialty(
            CreateSpecialtyCommand createSpecialty)
        {
            return await _mediator.Send(createSpecialty);
        }
    }
}
