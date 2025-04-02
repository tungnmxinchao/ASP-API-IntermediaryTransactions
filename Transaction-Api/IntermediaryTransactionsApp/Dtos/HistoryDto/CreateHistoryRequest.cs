using IntermediaryTransactionsApp.Db.Models;
using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.HistoryDto
{
	public class CreateHistoryRequest
	{

		[Required]
		public decimal Amount { get; set; }

		[Required]
		[Range(1, 3)]
		public int TransactionType { get; set; }
		[Required]
		public string Note { get; set; }
		[Required]
		public string Payload { get; set; }
		[Required]
		public int UserId { get; set; }
		[Required]
		public string OnDoneLink { get; set; }
	}
}
