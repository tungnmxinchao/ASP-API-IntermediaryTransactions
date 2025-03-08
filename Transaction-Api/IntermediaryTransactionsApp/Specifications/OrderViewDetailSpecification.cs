using IntermediaryTransactionsApp.Db.Models;
using System.Linq.Expressions;

namespace IntermediaryTransactionsApp.Specifications
{
    public class OrderViewDetailSpecification : BaseSpecification<Order>
    {
        public OrderViewDetailSpecification(Guid orderId, int userId) : base(order => 
            order.Id == orderId && (
                // Nếu order chưa được mua thì ai cũng xem được (nhưng nội dung ẩn sẽ được xử lý ở service)
                order.Updateable ||
                // Nếu order đã được mua thì chỉ người mua và người bán mới xem được
                (!order.Updateable && (order.CreatedBy == userId || order.Customer == userId))
            ))
        {
            AddInclude(o => o.CreatedByUser);
            AddInclude(o => o.CustomerUser);
          
        }
    }
} 