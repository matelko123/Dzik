using Application.Identity.Tokens;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Auth.Jwt;

internal sealed class HttpContextTokenStorage : ITokenStorage
{
    private const string TokenKey = "jwt";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTokenStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Set(string jwt) => _httpContextAccessor.HttpContext?.Items.TryAdd(TokenKey, jwt);

    public string? Get()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        if (_httpContextAccessor.HttpContext.Items.TryGetValue(TokenKey, out var jwt))
        {
            return jwt as string;
        }

        return null;
    }
}