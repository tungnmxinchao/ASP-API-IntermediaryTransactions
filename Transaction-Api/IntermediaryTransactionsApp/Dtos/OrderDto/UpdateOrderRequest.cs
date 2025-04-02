namespace IntermediaryTransactionsApp.Dtos.OrderDto
{
	public class UpdateOrderRequest : CreateOrderRequest
	{
		public Guid OrderId { get; set; }
	}
}
