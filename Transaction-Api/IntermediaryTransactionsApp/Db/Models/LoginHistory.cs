using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class LoginHistory
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public virtual User User { get; set; }

		[Required]
		[MaxLength(45)]
		public string IPAddress { get; set; }

		public DateTime LoginTime { get; set; } = DateTime.Now;
		public string DeviceInfo { get; set; }
	}
}
