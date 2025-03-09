using AutoMapper;
using IntermediaryTransactionsApp.Constants;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using Microsoft.EntityFrameworkCore;

namespace IntermediaryTransactionsApp.Service
{
	public class MessageService : IMessageService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly JwtService _jwtService;

		public MessageService(IMapper mapper, ApplicationDbContext context, 
			JwtService jwtService)
		{
			_jwtService = jwtService;
			_mapper = mapper;
			_context = context;
		}

		public async Task<bool> CreateMessage(CreateMessageRequest createMessageRequest)
		{
			if(createMessageRequest == null)
			{
				throw new ObjectNotFoundException("Object message not found.");
			}
			var message = _mapper.Map<Message>(createMessageRequest);

			message.Level = "info";
			message.OpenUrl = "example.com";
			message.Payload = "json";
			message.UserId = createMessageRequest.UserId;


			await _context.AddAsync(message);
			
			return true;
		}

        public async Task<List<Message>> GetMessages()
        {
            var userId = _jwtService.GetUserIdFromToken();
            if (userId == null)
            {
                throw new UnauthorizedAccessException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFoundInToken));
            }

            return await _context.Messages
                .Where(i => i.UserId == userId)
                .ToListAsync();

        }
    }
}
