using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class Message
	{
		public int Id { get; set; }

		public bool IsDelete { get; set; } = false;
		public int? CreatedBy { get; set; }
		public int? DeletedBy { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime? UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }

		[Required]
		public string Subject { get; set; }

		[Required]
		public string Content { get; set; }

		public bool Seen { get; set; } = false;

		[Required]
		[MaxLength(50)]
		[RegularExpression("info|warning|error")]
		public string Level { get; set; }

		public string OpenUrl { get; set; }
		public DateTime? ExpiredAt { get; set; }
		public bool Read { get; set; } = false;
		public string Payload { get; set; }

		public int UserId { get; set; }
		public virtual User User { get; set; }
	}
}
