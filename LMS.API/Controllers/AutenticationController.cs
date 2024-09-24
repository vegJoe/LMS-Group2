﻿using LMS.API.Models.Dtos;
using LMS.API.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;


/// <summary>
/// Handles user authentication and registration
/// </summary>
[Route("api/authentication")]
[ApiController]
public class AutenticationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AutenticationController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="userForRegistration">The user registration details</param>
    /// <returns>Returns 201 Created if the registration is successful, otherwise returns a bad request with the errors</returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var result = await _serviceManager.AuthService.RegisterUserAsync(userForRegistration);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created) : BadRequest(result.Errors);
    }

    /// <summary>
    /// Authenticates an existing user and generates a token
    /// </summary>
    /// <param name="user">The user's authentication details</param>
    /// <returns>Returns an authentication token if successful, or 401 Unauthorized if validation fails</returns>
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

