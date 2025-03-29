using IntermediaryTransactionsApp.Db.Models;
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

        public Task<bool> ResolveDispute(DisputeRequest request);

        public Task<OrderDetailResponse> GetOrderDetail(Guid orderId);

        public Task<List<OrdersPublicResponse>> GetOrdersPublic();

        public Task<List<Order>> GetMySaleOrders();

        public Task<List<MyPurchase>> GetMyPurchaseOrders();

        public Task<List<AdminGetOrderResponse>> FindAll();
    }
}
