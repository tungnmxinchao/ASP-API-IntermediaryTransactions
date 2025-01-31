using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Db.Models
{
	public class Role
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string RoleName { get; set; }

		public string Description { get; set; }

		public virtual ICollection<Users> Users { get; set; }
	}
}
