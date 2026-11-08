namespace Dif.Common.UseCaseHandling.Handlers;

public interface ICommand : IUseCase
{
    string CommandType { get; }
}
