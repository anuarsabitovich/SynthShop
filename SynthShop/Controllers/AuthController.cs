using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SynthShop.Domain.Entities;
using SynthShop.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using SynthShop.Core.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public AuthController(UserManager<User> userManager, IConfiguration configuration, IMapper mapper, IAuthService authService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest registerRequest
        )
    
    {
        var user = _mapper.Map<User>(registerRequest);

        var result = _authService.RegisterUserAsync(user, registerRequest.Password);
        
        return Ok(result.Result);
    }





    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest loginRequest)
    {

        var result = await _authService.SignInUserAsync(loginRequest);

        
        
        return Ok( result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshRequest)
    {
        var authResponse = await _authService.RefreshTokenAsync(refreshRequest.Token, refreshRequest.RefreshToken);

        return Ok(authResponse);
    }
        
}



