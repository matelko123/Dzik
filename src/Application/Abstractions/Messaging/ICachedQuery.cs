namespace Application.Abstractions.Messaging;

public interface ICachedQuery
{
    string Key { get; }
    TimeSpan? Expiration { get; }
}

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery;