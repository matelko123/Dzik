using Application.Common.Interfaces;

namespace Application.Identity.Users;

public interface IUserService
{
    Task<bool> ExistsWithNameAsync(string name);
    Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null);
    Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null);
}