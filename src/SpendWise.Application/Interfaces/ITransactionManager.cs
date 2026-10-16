namespace SpendWise.Application.Interfaces;

public interface ITransactionManager
{
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
}