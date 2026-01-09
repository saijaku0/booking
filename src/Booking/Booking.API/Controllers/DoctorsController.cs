using Booking.Application.Doctor.Dtos;
using Booking.Application.Doctor.Queries.GetDoctors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DoctorsController(IMediator mediator) : ControllerBase
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
            var doctorsList = _mediator.Send(getDoctors);

            return Ok(doctorsList);
        } 
    }
}
