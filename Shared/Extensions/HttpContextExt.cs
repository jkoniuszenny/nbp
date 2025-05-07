using Microsoft.AspNetCore.Http;


namespace Shared.Extensions;

public static class HttpContextExt
{
    public static string GetToken(this IHttpContextAccessor httpContent)
    {
        string token =
           httpContent?.HttpContext?.Request?.Headers?.FirstOrDefault(f => f.Key.ToLower() == "authorization").Value
           .ToString()?
           .Replace("Bearer ", "") ?? string.Empty;

        return token;

    }
}

