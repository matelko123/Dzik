using FluentAssertions;
using Shared.Wrapper;
using System.Text.Json;

namespace Api.FunctionalTests.Extensions;

internal static class HttpResponseMessageExtensions
{
    static readonly JsonSerializerOptions options = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };

    internal static async Task<Result<T?>> GetResult<T>(this HttpResponseMessage message)
    {
        var stream = await message.Content.ReadAsStreamAsync();

        Result<T?>? result = await JsonSerializer.DeserializeAsync<Result<T?>>(stream, options);

        result.Should().NotBeNull();

        return result!;
    }

    internal static async Task<Result> GetResult(this HttpResponseMessage message)
    {
        var stream = await message.Content.ReadAsStreamAsync();

        Result? result = await JsonSerializer.DeserializeAsync<Result>(stream, options);

        result.Should().NotBeNull();

        return result!;
    }
}
