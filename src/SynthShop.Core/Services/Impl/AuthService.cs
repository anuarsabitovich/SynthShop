using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Constants;
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
        private readonly IUnitOfWork _unitOfWork;
        
        public AuthService(UserManager<User> userManager, IConfiguration configuration, TokenValidationParameters tokenValidationParameters, IAuthRepository authRepository, ILogger logger, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _authRepository = authRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                _logger.Information("User {Email} registered successfully", user.Email);
                await _userManager.AddToRoleAsync(user, RoleConstants.User);
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

            var claims = await GetClaimListAsync(_userManager, user);

            var tokenResult = await GenerateAuthenticationResultAsync(user, claims);
            _logger.Information("User {Email} signed in successfully", loginRequest.Email);
            return tokenResult;
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, Guid refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

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
            await _unitOfWork.SaveChangesAsync();

            var userIdClaim = validatedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.Error("Claim {ClaimType} not found in token", ClaimTypes.NameIdentifier);
                return new AuthenticationResult() { Errors = new[] { "Invalid token" } };
            }

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null)
            {
                _logger.Warning("User with ID {UserId} not found", userIdClaim.Value);
                return new AuthenticationResult() { Errors = new[] { "Invalid token" } };
            }

            _logger.Information("Token refreshed successfully for user {UserId}", user.Id);
            var claims = await GetClaimListAsync(_userManager, user);

            return await GenerateAuthenticationResultAsync(user, claims);
        }


        private ClaimsPrincipal? GetPrincipalFromToken(string token)
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

        private async Task<AuthenticationResult> GenerateAuthenticationResultAsync(User user, List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            
            await _authRepository.AddRefreshToken(refreshToken);
            await _unitOfWork.SaveChangesAsync();
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

        private async Task<List<Claim>> GetClaimListAsync(UserManager<User> userManager, User user )
        {
            var userRoles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName),
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Sub, user.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            
            return claims;
        }

    }
}
