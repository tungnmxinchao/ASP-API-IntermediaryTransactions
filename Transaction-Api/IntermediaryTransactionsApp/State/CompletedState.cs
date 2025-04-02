
namespace IntermediaryTransactionsApp.State
{
    public class CompletedState : IOrderState
    {
        public Task CallAdmin(OrderContext context)
        {
            throw new InvalidOperationException("Không thể yêu cầu admin xử lý đơn hàng.");
        }

        public Task CancelOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể hủy đơn hàng.");
        }

        public Task Complain(OrderContext context)
        {
            throw new InvalidOperationException("Đơn hàng đã hoàn thành, không thể hoàn thành lại.");
        }

        public Task CompleteOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể khiếu nại đơn hàng đã hoàn thành.");
        }

        public Task RequestBuyerCheckOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể yêu cầu khách kiểm tra hàng");
        }
    }
}
