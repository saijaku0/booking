using Booking.Application.Reviews.Commands.CreateReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController(IMediator mediatR) : ControllerBase
    {
        private readonly IMediator _mediatR = mediatR;

        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewCommand request)
        {
            await _mediatR.Send(request);
            return Ok();
        }
    }
}
