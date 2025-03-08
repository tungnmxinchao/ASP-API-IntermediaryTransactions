using IntermediaryTransactionsApp.Db.Models;

namespace IntermediaryTransactionsApp.State
{
    public class OrderContext
    {
        private IOrderState _currentState;
        public Order Order { get; private set; }

        public OrderContext(Order order)
        {
            Order = order;
           
            SetStateBasedOnStatus();
        }

        public void SetState(IOrderState state)
        {
            _currentState = state;
        }

        private void SetStateBasedOnStatus()
        {
            switch (Order.StatusId)
            {
                case (int)OrderState.BuyerInspecting:
                    _currentState = new BuyerInspectingState();
                    break;
                case (int)OrderState.Completed:
                    _currentState = new CompletedState();
                    break;
                case (int)OrderState.BuyerComplained:
                    _currentState = new BuyerComplainedState();
                    break;
                case (int)OrderState.AwaitingBuyerConfirmation:
                    _currentState = new AwaitingBuyerConfirmationState();
                    break;
                case (int)OrderState.AdminMediationRequested:
                    _currentState = new AdminMediationRequestedState();
                    break;
                case (int)OrderState.Canceled:
                    _currentState = new AdminMediationRequestedState();
                    break;
                default:
                    _currentState = new BuyerInspectingState(); 
                    break;
            }
        }

        public Task CompleteOrder() => _currentState.CompleteOrder(this);
        public Task Complain() => _currentState.Complain(this);
        public Task RequestBuyerCheckOrder() => _currentState.RequestBuyerCheckOrder(this);
        public Task CallAdmin() => _currentState.CallAdmin(this);
        public Task CancelOrder() => _currentState.CancelOrder(this);
    }
}
