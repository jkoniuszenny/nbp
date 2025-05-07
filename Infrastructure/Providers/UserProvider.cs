using Application.Interfaces.Providers;
using Microsoft.AspNetCore.Http;
using Shared.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Providers;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContext;

    public UserProvider(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public string UserName
    {
        get
        {
            return GetJwtSecurityToken().GetClaims("username", "Missing username");
        }
    }

    private JwtSecurityToken GetJwtSecurityToken()
    {
        string token = _httpContext.GetToken();

        return token.Length == 0 || !token.Contains("Bearer",StringComparison.CurrentCultureIgnoreCase) 
            ? new JwtSecurityToken() 
            : new JwtSecurityTokenHandler().ReadJwtToken(token);
    }
}

