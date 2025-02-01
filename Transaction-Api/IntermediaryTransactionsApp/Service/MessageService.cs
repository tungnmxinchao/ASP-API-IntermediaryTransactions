using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.MessageInterface;

namespace IntermediaryTransactionsApp.Service
{
	public class MessageService : IMessageService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public MessageService(IMapper mapper, ApplicationDbContext context)
		{
			this._mapper = mapper;
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
	}
}
