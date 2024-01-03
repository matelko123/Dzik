using MediatR;
using Shared.Wrapper;

namespace Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}