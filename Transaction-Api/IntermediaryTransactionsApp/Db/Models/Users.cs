using System.ComponentModel.DataAnnotations;
using System.Data;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class Users
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Username { get; set; }

		[Required]
		[MaxLength(255)]
		public string PasswordHash { get; set; }

		[Required]
		[MaxLength(255)]
		public string Email { get; set; }

		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime? UpdatedAt { get; set; }

		public int RoleId { get; set; }
		public virtual Role Role { get; set; }
		public virtual ICollection<LoginHistory> LoginHistories { get; set; }
		public virtual ICollection<Order> CreatedOrders { get; set; }
		public virtual ICollection<Order> CustomerOrders { get; set; }
		public virtual ICollection<Message> Messages { get; set; }
		public virtual ICollection<TransactionHistory> TransactionHistories { get; set; }
	}
}
