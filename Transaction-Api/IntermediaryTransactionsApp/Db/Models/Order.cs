using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class Order
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public bool IsDelete { get; set; } = false;
		public int CreatedBy { get; set; }
		public Users CreatedByUser { get; set; }
		public int? DeletedBy { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime? UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }

		[Required]
		public string Contact { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }
		public bool IsPublic { get; set; } = true;
		public string HiddenValue { get; set; }

		[Required]
		public decimal MoneyValue { get; set; }

		public int StatusId { get; set; }
		public virtual OrderStatus Status { get; set; }

		public bool IsSellerChargeFee { get; set; } = false;
		public bool IsPaidToSeller { get; set; } = false;

		public int? Customer { get; set; }
		public virtual Users CustomerUser { get; set; }

		public string ShareLink { get; set; }

		public decimal FeeOnSuccess { get; set; }
		public decimal TotalMoneyForBuyer { get; set; }
		public decimal SellerReceivedOnSuccess { get; set; }

		public bool Updateable { get; set; } = false;
		public bool CustomerCanComplain { get; set; } = false;
	}
}
