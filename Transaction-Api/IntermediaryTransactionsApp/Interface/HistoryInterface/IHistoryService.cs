using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;

namespace IntermediaryTransactionsApp.Interface.HistoryInterface
{
	public interface IHistoryService
	{
		public Task<bool> CreateHistory(CreateHistoryRequest historyRequest);

		public Task<List<TransactionHistory>> GetHistoryTransactions();

		public Task<List<AdminTransactionHistory>> FindAll();

        public Task<bool> UpdateHistory(int historyId, bool isProcess);

    }
}
