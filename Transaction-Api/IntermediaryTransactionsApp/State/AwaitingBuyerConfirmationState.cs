
using IntermediaryTransactionsApp.Specifications;

namespace IntermediaryTransactionsApp.State
{
    public class AwaitingBuyerConfirmationState : IOrderState
    {
        public async Task CallAdmin(OrderContext context)
        {
            context.Order.StatusId = (int)OrderState.AdminMediationRequested;
            context.SetState(new AdminMediationRequestedState());
            await Task.CompletedTask;
        }

        public Task CancelOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể hủy đơn hàng.");
        }

        public Task Complain(OrderContext context)
        {
            throw new InvalidOperationException("Không thể khiếu nại đơn hàng.");
        }

        public async Task CompleteOrder(OrderContext context)
        {
            context.Order.StatusId = (int)OrderState.Completed;
            context.SetState(new CompletedState());
            await Task.CompletedTask;
        }

        public async Task RequestBuyerCheckOrder(OrderContext context)
        {
            throw new InvalidOperationException("Đã yêu cầu khách kiểm tra lại đơn hàng.");
        }
    }
}
