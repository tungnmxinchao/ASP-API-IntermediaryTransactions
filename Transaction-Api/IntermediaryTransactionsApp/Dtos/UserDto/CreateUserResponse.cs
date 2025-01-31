using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.UserDto
{
	public class CreateUserResponse
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public int RoleId { get; set; }
	}
}
