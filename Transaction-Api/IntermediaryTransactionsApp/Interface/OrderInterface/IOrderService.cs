using IntermediaryTransactionsApp.Dtos.OrderDto;

namespace IntermediaryTransactionsApp.Interface.IOrderService
{
	public interface IOrderService
	{
		public Task<bool> CreateOrder(CreateOrderRequest request);

		public Task<UpdateOrderResponse> UpdateOrder(UpdateOrderRequest request);

        public Task<bool> BuyOrder(BuyOrderRequest request);

        public Task<bool> CompleteOrder(Guid orderId);

        public Task<bool> ComplainOrder(Guid orderId);

        public Task<bool> RequestBuyerCheckOrder(Guid orderId);

        public Task<bool> CallAdmin(Guid orderId);

        public Task<bool> CancelOrder(Guid orderId);
    }
}
