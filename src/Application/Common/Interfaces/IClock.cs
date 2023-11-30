namespace Application.Common.Interfaces;

public interface IClock : ISingletonService
{
    DateTime Current();
}