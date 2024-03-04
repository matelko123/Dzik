using Contracts.Common;
using System.Net;

namespace Api.FunctionalTests.Extensions;

internal static class ResultsExtensions
{
    internal static ErrorResult ValidationErrorResult(Dictionary<string, string[]> errors) => new ()
    {
        StatusCode = (int)HttpStatusCode.UnprocessableEntity,
        Message = "One or more validation errors occurs",
        Exception = "FluentValidation.ValidationException",
        Errors = errors ?? []
    };
}
