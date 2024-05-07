using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using SynthShop.Domain.Entities;
using AuthenticationResult = SynthShop.Domain.Results.AuthenticationResult;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<AuthenticationResult> SignInUserAsync(LoginRequest loginRequest);
        Task<AuthenticationResult> RefreshTokenAsync(string token, Guid refreshToken);   
    }

}
