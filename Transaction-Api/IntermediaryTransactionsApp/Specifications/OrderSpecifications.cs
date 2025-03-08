using IntermediaryTransactionsApp.Db.Models;

namespace IntermediaryTransactionsApp.Specifications
{
    public class OrderByIdSpecification : BaseSpecification<Order>
    {
        public OrderByIdSpecification(Guid orderId) 
            : base(order => order.Id == orderId)
        {
        }
    }

    public class OrderByUserIdSpecification : BaseSpecification<Order>
    {
        public OrderByUserIdSpecification(int userId) 
            : base(order => order.CreatedBy == userId)
        {
        }
    }

    public class OrderByStatusSpecification : BaseSpecification<Order>
    {
        public OrderByStatusSpecification(int statusId) 
            : base(order => order.StatusId == statusId)
        {
        }
    }

    public class OrderUpdateableSpecification : BaseSpecification<Order>
    {
        public OrderUpdateableSpecification(Guid orderId, int userId) 
            : base(order => order.Id == orderId && 
                          order.CreatedBy == userId && 
                          order.Updateable == true)
        {
        }
    }

    public class OrderBuyableSpecification : BaseSpecification<Order>
    {
        public OrderBuyableSpecification(Guid orderId, int userId) 
            : base(order => order.Id == orderId && 
                          order.CreatedBy != userId && 
                          order.StatusId == 1)
        {
        }
    }

    public class OrderCompletableSpecification : BaseSpecification<Order>
    {
        public OrderCompletableSpecification(Guid orderId, int userId)
            : base(order => order.Id == orderId &&
                          order.Customer == userId &&
                          (order.StatusId == 3 || order.StatusId == 8))
        {
        }
    }

    public class OrderComplaintSpecification : BaseSpecification<Order>
    {
        public OrderComplaintSpecification(Guid orderId, int userId)
            : base(order => order.Id == orderId &&
                          order.Customer == userId &&
                          order.StatusId == 3)
        {
        }
    }

    public class RequestBuyerCheckOrder : BaseSpecification<Order>
    {
        public RequestBuyerCheckOrder(Guid orderId, int userId)
            : base(order => order.Id == orderId &&
                          order.CreatedBy == userId &&
                          order.StatusId == 5)
        {
        }
    }

    public class CallAdminHandleOrder : BaseSpecification<Order>
    {
        public CallAdminHandleOrder(Guid orderId)
            : base(order => order.Id == orderId &&
                          (order.StatusId == 5 || order.StatusId == 8 ))
        {
        }
    }

    public class CancelOrder : BaseSpecification<Order>
    {
        public CancelOrder(Guid orderId, int userId)
            : base(order => order.Id == orderId && (order.CreatedBy == userId) &&
                          (order.StatusId == 5 || order.StatusId == 7))
        {
        }
    }
} 