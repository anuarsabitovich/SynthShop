using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;
using AuthenticationResult = SynthShop.Domain.Results.AuthenticationResult;

namespace SynthShop.Core.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger _logger;
        
        public AuthService(UserManager<User> userManager, IConfiguration configuration, TokenValidationParameters tokenValidationParameters, IAuthRepository authRepository, ILogger logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _authRepository = authRepository;
            _logger = logger;
        }
        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                _logger.Information("User {Email} registered successfully", user.Email);
            }
            else
            {
                _logger.Error("User registration failed for {Email}. Errors: {@Errors}", user.Email, result.Errors);
            }
            return result;
        }



        public async Task<AuthenticationResult> SignInUserAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                _logger.Warning("Login failed for email {Email}: user not found", loginRequest.Email);
                return new AuthenticationResult() { Errors = new[] { "Login Failed" } };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!result)
            {
                _logger.Warning("Invalid password attempt for user {Email}", loginRequest.Email);
                return new AuthenticationResult() { Errors = new[] { "Login Failed" } };
            }

            var tokenResult = GenerateAuthenticationResultForUser(user);
            _logger.Information("User {Email} signed in successfully", loginRequest.Email);
            return await tokenResult;
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, Guid refreshToken)
        {
            var validatedToken =  getPrincipalFromToken(token);

            if (!ValidateAccessToken(validatedToken))
            {
                return new AuthenticationResult() { Errors = new[] { "Invalid token" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _authRepository.GetRefreshTokenById(refreshToken);

            if (!ValidateRefreshToken(storedRefreshToken, jti))
            {
                return new AuthenticationResult { Errors = new[] { "Invalid refresh token" } };
            }

            storedRefreshToken.Used = true;
            await _authRepository.UpdateRefreshToken(storedRefreshToken);

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            _logger.Information("Token refreshed successfully for user {UserId}", user.Id);
            return await GenerateAuthenticationResultForUser(user);
        }

        private ClaimsPrincipal getPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString())
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

            var refreshToken = await _authRepository.AddRefreshToken(new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            });

            return new AuthenticationResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken.Token.ToString()
            };
        }

        private bool ValidateAccessToken(ClaimsPrincipal validatedToken)
        {
            
            if (validatedToken == null)
            {
                return false;
            }
            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return false;   
            }

            return true;
        }

        private bool ValidateRefreshToken(RefreshToken storedRefreshToken, string jti)
        {
            if (storedRefreshToken == null)
            {
                return false;
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return false;

            }

            if (storedRefreshToken.Invalidated)
            {
                return false;
            }

            if (storedRefreshToken.Used)
            {
                return false;
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return false;
            }

            return true;
        }
    }
}
