namespace Application.Errors;

public static class UserErrors
{
    public const string NotFound = "User not found.";
    public const string LockedOut = "Your account is locked out. Please contact with administrator.";
    public const string NotAllowed = "You cannot sign into your account. Please contact with administrator.";

    public static class Validation
    {
        public static class FirstName
        {
            public const int MinimumLength = 5;
            public static readonly string MinimumLengthMessage = $"UserName should be at least {MinimumLength} characters";
        }
        
        public static class LastName
        {
            public const int MinimumLength = 5;
            public static readonly string MinimumLengthMessage = $"LastName should be at least {MinimumLength} characters";
        }
        
        public static class Email
        {
            public const string NotEmpty = "Email cannot be empty";
            public const string InvalidFormat = "'Email' is not a valid email address.";
            public static string AlreadyTaken(string email) => $"Email {email} is already registered.";
        }

        public static class Username
        {
            public const int MinimumLength = 5;
            
            public const string NotEmpty = "Username cannot be empty";
            public static readonly string MinimumLengthMessage = $"Username should be at least {MinimumLength} characters";
            public static string AlreadyTaken(string name) => $"Username {name} already taken";
        }

        public static class Password
        {
            public const string NotEmpty = "Password cannot be empty";
            public static string MinimumLengthMessage(int length) => $"Password should be at least {length} characters";
        }

        public static class PhoneNumber
        {
            public const string NotEmpty = "Phone number cannot be empty";
            public const string InvalidFormat = "Invalid phone number format.";
            public static string AlreadyTaken(string phone) => $"Phone number {phone} is already registered.";
        }
    }
}