using System.Security.Claims;
using SynthShop.Core.Services.Interfaces;

namespace SynthShop.Extensions
{
    public class UserProvider: IUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
