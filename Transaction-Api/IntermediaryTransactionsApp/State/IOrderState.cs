namespace IntermediaryTransactionsApp.State
{
    public interface IOrderState
    {
        Task CompleteOrder(OrderContext context);
        Task Complain(OrderContext context);
        Task RequestBuyerCheckOrder(OrderContext context);
        Task CallAdmin(OrderContext context);
        Task CancelOrder(OrderContext context);
    }
}
