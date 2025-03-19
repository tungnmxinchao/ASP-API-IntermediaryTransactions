namespace IntermediaryTransactionsApp.Dtos.OrderDto
{
    public class DisputeRequest
    {
        public Guid OrderId { get; set; }
        public bool isSellerCorrect { get; set; }

    }
}
