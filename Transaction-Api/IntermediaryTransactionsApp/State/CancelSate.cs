
namespace IntermediaryTransactionsApp.State
{
    public class CancelSate : IOrderState
    {
        public Task CallAdmin(OrderContext context)
        {
            throw new InvalidOperationException("Không thể yêu cầu admin xử lý đơn hàng.");
        }

        public Task CancelOrder(OrderContext context)
        {
            throw new InvalidOperationException("Đơn hàng không thể hủy.");
        }

        public Task Complain(OrderContext context)
        {
            throw new InvalidOperationException("Đơn không thể đã được khiếu nại.");
        }

        public Task CompleteOrder(OrderContext context)
        {
            throw new InvalidOperationException("Đơn hàng không thể hoàn thành.");
        }

        public Task RequestBuyerCheckOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể yêu cách khách hàng kiểm trang lại đơn hàng");
        }
    }
}
