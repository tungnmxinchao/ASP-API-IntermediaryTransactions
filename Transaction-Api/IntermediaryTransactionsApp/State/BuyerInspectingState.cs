
using IntermediaryTransactionsApp.Db.Models;

namespace IntermediaryTransactionsApp.State
{
    public class BuyerInspectingState : IOrderState
    {
        public Task CallAdmin(OrderContext context)
        {
            throw new InvalidOperationException("Không thể yêu cầu admin xử lý đơn hàng");
        }

        public Task CancelOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể hủy đơn hàng");
        }

        public async Task Complain(OrderContext context)
        {
            context.Order.StatusId = (int)OrderState.BuyerComplained;
            context.SetState(new BuyerComplainedState());
            await Task.CompletedTask;

        }

        public async Task CompleteOrder(OrderContext context)
        {
            context.Order.StatusId = (int)OrderState.Completed;
            context.SetState(new CompletedState());
            await Task.CompletedTask;
        }

        public Task RequestBuyerCheckOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể yêu cầu khách kiểm tra hàng");
        }
    }
}
