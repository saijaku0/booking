using Booking.Application.Reviews.Commands.CreateReview;
using Booking.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReviewsController(IMediator mediatR) : ControllerBase
    {
        private readonly IMediator _mediatR = mediatR;

        /// <summary>
        /// Submits a review for a doctor.
        /// </summary>
        /// <remarks>
        /// Only patients can submit reviews. 
        /// The review must contain a rating (1-5) and a comment.
        /// </remarks>
        /// <param name="request">Review details (DoctorId, Rating, Text)</param>
        /// <returns>The ID of the created review</returns>
        /// <response code="200">Review submitted successfully.</response>
        /// <response code="400">Validation failed (e.g. rating out of range).</response>
        /// <response code="401">User is not authorized.</response>
        /// <response code="403">Only patients can leave reviews.</response>
        [HttpPost]
        [Authorize(Roles = Roles.Patient)] 
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateReview(CreateReviewCommand request)
        {
            await _mediatR.Send(request);
            return Ok();
        }

        // TO DO: add get reviews for doctor, but first you need to create ReviewDto and GetDoctorReviewsQuery
        /*/// <summary>
        /// Get all reviews for a specific doctor.
        /// </summary>
        /// <param name="doctorId">The unique identifier of the doctor</param>
        /// <returns>List of reviews with ratings and comments</returns>
        [HttpGet] 
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReviewDto>>> GetReviews(
            [FromQuery] Guid doctorId)
        {
            // Тебе нужно будет создать этот Query, если его еще нет
            var reviews = await _mediatR.Send(new GetDoctorReviewsQuery(doctorId));

            return Ok(reviews);
        }*/
    }
}
