using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.OrderDto
{
	public class CreateOrderRequest
	{
		[Required]
		public string Contact { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public bool IsPublic { get; set; }

		[Required]
		public string HiddenValue { get; set; }

		[Required]
		public decimal MoneyValue { get; set; }

		[Required]
		public bool IsSellerChargeFee { get; set; }
	}
}
