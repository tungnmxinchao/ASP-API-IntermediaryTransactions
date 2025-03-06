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
} 