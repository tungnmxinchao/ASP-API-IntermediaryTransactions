using IntermediaryTransactionsApp.Db.Models;
using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.MessageDto
{
	public class CreateMessageRequest
	{

		[Required]
		public string Subject { get; set; }
		[Required]
		public string Content { get; set; }
		[Required]
		public int UserId { get; set; }

        [Required]
        public Guid OrderId { get; set; }
    }
}
