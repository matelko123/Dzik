using Application.Abstractions.Messaging;
using Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Abstractions.Behaviors;

public class ValidationBehavior <TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        var context = new ValidationContext<TRequest>(request);

        var validationResults =  await Task.WhenAll(_validators
            .Select(x => x.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(r => r.Errors)
            .Select(validationFailure =>
                new ValidationError(
                    validationFailure.PropertyName,
                    validationFailure.ErrorMessage))
            .ToList();

        if (failures.Count != 0)
        {
            throw new Exceptions.ValidationException(failures);
        }
            
        return await next();
    }
}