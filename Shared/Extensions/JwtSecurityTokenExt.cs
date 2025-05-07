using System.IdentityModel.Tokens.Jwt;

namespace Shared.Extensions;

public static class JwtSecurityTokenExt
{
    public static string GetClaims(this JwtSecurityToken token, string claimsName, string ifNotFoundReturn = "")
    {
        return token?.Claims?.FirstOrDefault(s => s.Type.ToLower() == claimsName.ToLower())?.Value ?? ifNotFoundReturn;
    }
}

