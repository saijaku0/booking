using Booking.Application.Identity.Commands.LoginUser;
using Booking.Application.Identity.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Registration new user
        /// </summary>
        /// <remarks>
        /// Creating new user. Password must be with (numbers, letters, symbols)
        /// </remarks>
        /// <param name="registerUserCommand">Registration data (Email, password, name, surename)</param>
        /// <returns>New user ID</returns>
        /// <response code="200">Success, new user created</response>
        /// <response code="400">Errore of validation (password to short, busy email)</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerUserCommand)
        {
            var userId = await _mediator.Send(registerUserCommand);

            return Ok(new { UserId = userId});
        }

        /// <summary>
        /// Sign in (get token)
        /// </summary>
        /// <param name="command">Email and password</param>
        /// <returns>JWT token for access</returns>
        /// <response code="200">Login is success</response>
        /// <response code="400">Errore invalid email or password</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var token = await _mediator.Send(command);

            return Ok(new { Token = token });
        }
    }
}
