using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.HistoryInterface;

namespace IntermediaryTransactionsApp.Service
{
	public class HistoryService : IHistoryService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public HistoryService( ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<bool> CreateHistory(CreateHistoryRequest historyRequest)
		{
			if (historyRequest == null)
			{
				throw new ObjectNotFoundException("Object history request not found");
			}

			var history = _mapper.Map<TransactionHistory>(historyRequest);
			history.CreatedBy = historyRequest.UserId;
			history.IsProcessed = true;
			history.OnDoneAction = "";

			await _context.AddAsync(history);
			
			return true;

		}
	}
}
