namespace IntermediaryTransactionsApp.Dtos.OrderDto
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public int StatusId { get; set; }
        public string Contact { get; set; }
        public string? HiddenValue { get; set; }
        public decimal MoneyValue { get; set; }
        public bool IsSellerChargeFee { get; set; }
        public decimal FeeOnSuccess { get; set; }
        public decimal TotalMoneyForBuyer { get; set; }
        public decimal SellerReceivedOnSuccess { get; set; }
        public bool Updateable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ShareLink { get; set; }
        public UserBasicInfoDto? Customer { get; set; }
        public UserBasicInfoDto CreatedByUser { get; set; }
    }

    public class UserBasicInfoDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
} 