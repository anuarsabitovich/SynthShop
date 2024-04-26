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

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest registerRequest,
        [FromServices] IServiceProvider sp)
    {
        var userManager = sp.GetRequiredService<UserManager<User>>();


        var userStore = sp.GetRequiredService<IUserStore<User>>();
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = registerRequest.Email;



        var user = new User();
        user.FirstName = registerRequest.FirstName;
        user.LastName = registerRequest.LastName;
        user.Address = registerRequest.Address;
        user.Email = registerRequest.Email;
        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok();
    }





    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (!result)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        // User authenticated, generate JWT token
        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }


    // will be removed from here 

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



