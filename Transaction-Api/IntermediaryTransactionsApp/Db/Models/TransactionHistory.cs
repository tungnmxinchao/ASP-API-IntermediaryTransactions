using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class TransactionHistory
	{
		public int Id { get; set; }

		public bool IsDelete { get; set; } = false;
		public int CreatedBy { get; set; }
		public int? DeletedBy { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public DateTime? DeletedAt { get; set; }

		[Required]
		public decimal Amount { get; set; }

		[Required]
		[Range(1, 3)]
		public int TransactionType { get; set; }

		public bool IsProcessed { get; set; } = false;
		public string Note { get; set; }
		public string OnDoneAction { get; set; }
		public string Payload { get; set; }

		public int UserId { get; set; }
		public virtual Users User { get; set; }
		public string OnDoneLink { get; set; }
	}
}
