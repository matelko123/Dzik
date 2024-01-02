namespace Application.Errors;

public static class UserErrors
{
    public const string NotFound = "User not found.";
    public const string LockedOut = "Your account is locked out. Please contact with administrator.";
    public const string NotAllowed = "You cannot sign in to your account. Please contact with administrator.";

    public static class Validation
    {
        public static string EmailAlreadyTaken(string email) => $"Email {email} is already registered.";
        public static string UsernameAlreadyTaken(string name) => $"Username {name} already taken";
        public static string PhoneNumberAlreadyTaken(string phone) => $"Phone number {phone} is already registered.";
        
        public const string InvalidEmail = "'Email' is not a valid email address.";
        public const string InvalidPhoneNumberFormat = "Invalid phone number format.";
    }
}