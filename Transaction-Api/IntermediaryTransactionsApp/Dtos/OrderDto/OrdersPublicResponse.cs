namespace IntermediaryTransactionsApp.Dtos.OrderDto
{
    public class OrdersPublicResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public decimal MoneyValue { get; set; }
        public bool IsSellerChargeFee { get; set; }
        public decimal FeeOnSuccess { get; set; }
        public decimal TotalMoneyForBuyer { get; set; }
        public decimal SellerReceivedOnSuccess { get; set; }
        public bool Updateable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserBasicInfoDto? CustomerUser { get; set; }
        public UserBasicInfoDto CreatedByUser { get; set; }
    }
}
