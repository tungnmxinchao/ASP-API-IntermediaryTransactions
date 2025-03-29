using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Dtos.UserDto;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace IntermediaryTransactionsApp.Config
{
    public class EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<TransactionHistory>("TransactionHistory");
            builder.EntitySet<Message>("Message");
            builder.EntitySet<Order>("MyOrder");
            builder.EntitySet<MyPurchase>("MyPurchase");

            var user = builder.EntitySet<GetUserResponse>("Users").EntityType;
            user.HasKey(u => u.Id);

            var adminGetOrders = builder.EntitySet<AdminGetOrderResponse>("AdminViewOrders").EntityType;
            adminGetOrders.HasKey(o => o.Id);

            var adminGetTransactionHistories = builder.EntitySet<AdminTransactionHistory>("AdminViewTransactions").EntityType;
            adminGetOrders.HasKey(o => o.Id);


            ConfigureOrderEntity(builder);
            ConfigureUserEntity(builder);

            return builder.GetEdmModel();
        }

        private static void ConfigureOrderEntity(ODataConventionModelBuilder builder)
        {
           
            var orderEntity = builder.EntitySet<OrdersPublicResponse>("Order").EntityType;

            orderEntity.HasKey(o => o.Id);
            orderEntity.Property(o => o.Title);
            orderEntity.Property(o => o.Description);
            orderEntity.Property(o => o.IsPublic);
            orderEntity.Property(o => o.MoneyValue);
            orderEntity.Property(o => o.IsSellerChargeFee);
            orderEntity.Property(o => o.FeeOnSuccess);
            orderEntity.Property(o => o.TotalMoneyForBuyer);
            orderEntity.Property(o => o.SellerReceivedOnSuccess);
            orderEntity.Property(o => o.Updateable);
            orderEntity.Property(o => o.CreatedAt);
            orderEntity.Property(o => o.UpdatedAt);

       
            orderEntity.HasOptional(o => o.CustomerUser); 
            orderEntity.HasRequired(o => o.CreatedByUser);
        }

        private static void ConfigureUserEntity(ODataConventionModelBuilder builder)
        {
            var userEntity = builder.EntityType<UserBasicInfoDto>();

            userEntity.HasKey(u => u.Id);
            userEntity.Property(u => u.Username);
            userEntity.Property(u => u.Email);
        }
    }
}
