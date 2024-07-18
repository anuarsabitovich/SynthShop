using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Domain.Entities;
using SynthShop.DTO;
using AutoMapper;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Validations;
using ILogger = Serilog.ILogger;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;
    private readonly ILogger _logger;
    private readonly RegistrationRequestValidator _registrationRequestValidator;

    public AuthController(IMapper mapper, IAuthService authService, ILogger logger,
        RegistrationRequestValidator registrationRequestValidator)
    {
        _mapper = mapper;
        _authService = authService;
        _registrationRequestValidator = registrationRequestValidator;
        _logger = logger.ForContext<AuthController>();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest registerRequest)
    {
        var validationResult = _registrationRequestValidator.Validate(registerRequest);
        if (validationResult.IsValid == false)
        {
            _logger.Warning("Validation failed for creating user. Errors: {@ValidationErrors}",
                validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        var user = _mapper.Map<User>(registerRequest);

        var result = _authService.RegisterUserAsync(user, registerRequest.Password);

        if (result.Result.Succeeded)
        {
            return Ok(result.Result);
        }
        else
        {
            return BadRequest(result.Result.Errors);

        }

    }


    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest loginRequest)
    {
        _logger.Information("User with username {username} starting to login", loginRequest.Email);

        var result = await _authService.SignInUserAsync(loginRequest);

        if (result.Errors.Any())
        {
            _logger.Warning("User with username {username} failed to login", loginRequest.Email);
            return BadRequest(result.Errors);
        }

        _logger.Information("User {username} has successfully logged in", loginRequest.Email);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshRequest)
    {
        var authResponse = await _authService.RefreshTokenAsync(refreshRequest.Token, refreshRequest.RefreshToken);

        if (authResponse.Errors.Any()) return BadRequest(authResponse.Errors);
        return Ok(authResponse);
    }
}