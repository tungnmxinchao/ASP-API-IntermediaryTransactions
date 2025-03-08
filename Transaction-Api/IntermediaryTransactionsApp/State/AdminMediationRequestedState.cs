﻿
namespace IntermediaryTransactionsApp.State
{
    public class AdminMediationRequestedState : IOrderState
    {
        public Task CallAdmin(OrderContext context)
        {
            throw new InvalidOperationException("Không thể gọi admin xử lý.");
        }

        public Task CancelOrder(OrderContext context)
        {
            throw new NotImplementedException();
        }

        public Task Complain(OrderContext context)
        {
            throw new InvalidOperationException("Không thể khiếu nại đơn hàng.");
        }

        public Task CompleteOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể hoàn thành đơn hàng.");
        }

        public Task RequestBuyerCheckOrder(OrderContext context)
        {
            throw new InvalidOperationException("Không thể yêu cách khách hàng kiểm tra lại đơn hàng.");
        }
    }
}
