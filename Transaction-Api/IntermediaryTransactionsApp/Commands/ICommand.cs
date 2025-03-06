namespace IntermediaryTransactionsApp.Commands
{
    public interface ICommand<TResult>
    {
        Task<TResult> ExecuteAsync();
    }
} 