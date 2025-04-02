using AutoMapper;
using IntermediaryTransactionsApp.Constants;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using Microsoft.EntityFrameworkCore;

namespace IntermediaryTransactionsApp.Service
{
	public class HistoryService : IHistoryService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
        private readonly JwtService _jwtService;

        public HistoryService( ApplicationDbContext context, IMapper mapper, JwtService jwtService)
		{
			_jwtService = jwtService;
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

        public async Task<bool> UpdateHistory(int historyId, bool isProcess)
		{
			var history = _context.TransactionHistories.Find(historyId);

            if (history == null)
            {
                throw new ObjectNotFoundException("Object history request not found");
            }

			history.IsProcessed = isProcess;

			_context.Update(history);

			return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<AdminTransactionHistory>> FindAll()
        {
			var histories = await _context.TransactionHistories
				.Include(u => u.User)
                .ToListAsync();

			var response = _mapper.Map<List<AdminTransactionHistory>>(histories);

			return response;       

        }

        public async Task<List<TransactionHistory>> GetHistoryTransactions()
		{
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            return await _context.TransactionHistories
                .Where(i => i.UserId == userId)
                .ToListAsync(); 

        }
	}
}
