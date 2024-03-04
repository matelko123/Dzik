using Contracts.Common;
using System.Net;

namespace Application.Errors;

public static class UserErrors
{
    public static Error NotFound => new(HttpStatusCode.NotFound, "User not found.");
    public static Error WrongPassword => new(HttpStatusCode.Unauthorized, "Wrong password");
    public static Error LockedOut => new(HttpStatusCode.Unauthorized, "Your account is locked out. Please contact with administrator.");
    public static Error NotAllowed => new(HttpStatusCode.Unauthorized, "You cannot sign into your account. Please contact with administrator.");
    public static Error EmailNotConfirmed => new(HttpStatusCode.Unauthorized, "Please confirm your email.");


    public static class Validation
    {
        public const string EmailAlreadyTaken = "Email already taken.";
        public const string UsernameAlreadyTaken = "Username already taken.";
        public const string PhoneNumberInvalidFormat = "Invalid phone number format.";
        public const string PhoneNumberAlreadyTaken = "Phone number already taken.";
    }
}