using LMS.API.Models.Dtos;
using LMS.API.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;


[Route("api/authentication")]
[ApiController]
public class AutenticationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AutenticationController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var result = await _serviceManager.AuthService.RegisterUserAsync(userForRegistration);

        if (result.Succeeded)
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        var problemDetails = new ProblemDetails
        {
            Title = "User Registration Failed",
            Detail = "One or more errors occurred during user registration.",
            Status = StatusCodes.Status400BadRequest,
            Instance = HttpContext.Request.Path
        };

        // Add validation errors as an additional field
        problemDetails.Extensions.Add("errors", result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));

        return BadRequest(problemDetails);

    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate(UserForAuthenticationDto user)
    {
        if (!await _serviceManager.AuthService.ValidateUserAsync(user))
            return Unauthorized();

        TokenDto tokenDto = await _serviceManager.AuthService.CreateTokenAsync(expireTime: true);
        return Ok(tokenDto);
    }
}

