using System.Text;
using Application.Features.Identity.Authentication.Commands;

namespace Application.IntegrationTests.Identity.AuthEndpointsTests;

public static class RegisterCommandBuilder
{
    public static RegisterCommand Create() =>
        new RegisterCommand(
            GenerateRandomString(6), 
            GenerateRandomString(10), 
            GenerateRandomString(7), 
            GenerateRandomEmail(),
            GenerateRandomString(12), 
            GenerateRandomPhoneNumber());
    
    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        StringBuilder stringBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length);
            stringBuilder.Append(chars[index]);
        }

        return stringBuilder.ToString();
    }
    
    private static string GenerateRandomPhoneNumber() => new Random().Next(0, 999_999_99).ToString();
    private static string GenerateRandomEmail() => $"{GenerateRandomString(6)}@gmail.com";
}