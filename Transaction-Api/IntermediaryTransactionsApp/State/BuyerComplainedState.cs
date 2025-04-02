
namespace IntermediaryTransactionsApp.State
{
    public class BuyerComplainedState : IOrderState
    {
        public async Task CallAdmin(OrderContext context)
        {
            context.Order.StatusId = (int)OrderState.AdminMediationRequested;
            context.SetState(new AdminMediationRequestedState());
            await Task.CompletedTask;
        }

        public async Task CancelOrder(OrderContext context)
        {

            context.Order.StatusId = (int)OrderState.Canceled;
            context.SetState(new CancelSate());
            await Task.CompletedTask;
        }

        public Task Complain(OrderContext context)
        {
            throw new InvalidOperationException("Đơn hàng đã được khiếu nại.");
        }

        public Task CompleteOrder(OrderContext context)
        {
            throw new InvalidOperationException("Đơn hàng đang khiếu nại, không thể hoàn thành.");
        }

        public async Task RequestBuyerCheckOrder(OrderContext context)
        {
            context.Order.StatusId = (int)OrderState.AwaitingBuyerConfirmation;
            context.SetState(new AwaitingBuyerConfirmationState());
            await Task.CompletedTask;
        }
    }
}
