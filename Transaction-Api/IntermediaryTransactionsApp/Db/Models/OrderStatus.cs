using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class OrderStatus
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string StatusName { get; set; }

		[MaxLength(255)]
		public string Description { get; set; }

	}
}
