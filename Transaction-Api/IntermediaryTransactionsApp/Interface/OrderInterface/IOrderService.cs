using IntermediaryTransactionsApp.Dtos.OrderDto;

namespace IntermediaryTransactionsApp.Interface.IOrderService
{
	public interface IOrderService
	{
		public Task<bool> CreateOrder(CreateOrderRequest request);
	}
}
