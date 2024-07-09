using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SynthShop.Extensions;

public class RolesAttribute : AuthorizeAttribute
{
    public RolesAttribute(params string[] roles)
    {
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        Roles = string.Join(",", roles);
    }
}