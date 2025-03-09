using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;

namespace IntermediaryTransactionsApp.Interface.HistoryInterface
{
	public interface IHistoryService
	{
		public Task<bool> CreateHistory(CreateHistoryRequest historyRequest);

		public Task<List<TransactionHistory>> GetHistoryTransactions();

    }
}
