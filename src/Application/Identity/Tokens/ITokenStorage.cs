using Application.Common.Interfaces;

namespace Application.Identity.Tokens;

public interface ITokenStorage : ISingletonService
{
    void Set(string jwt);
    string? Get();
}