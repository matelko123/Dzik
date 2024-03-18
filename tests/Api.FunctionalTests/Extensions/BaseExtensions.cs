namespace Api.FunctionalTests.Extensions;

internal static class BaseExtensions
{
    private static readonly Random Random = new();
    internal static string RandomString(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    internal static string RandomNumeric(int length = 9)
        => Random.Next(1 * 10 ^ length, 9 * 10 ^ length).ToString();
}
