using IntermediaryTransactionsApp.Dtos.MessageDto;

namespace IntermediaryTransactionsApp.Interface.MessageInterface
{
	public interface IMessageService
	{
		public Task<bool> CreateMessage(CreateMessageRequest createMessageRequest);
	}
}
