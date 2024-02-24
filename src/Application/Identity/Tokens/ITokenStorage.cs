using Application.Common.Interfaces;

namespace Application.Identity.Tokens;

public interface ITokenStorage
{
    void Set(string jwt);
    string? Get();
}