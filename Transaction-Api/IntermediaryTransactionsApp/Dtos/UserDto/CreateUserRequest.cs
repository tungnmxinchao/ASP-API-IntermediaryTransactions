using System.ComponentModel.DataAnnotations;

namespace IntermediaryTransactionsApp.Dtos.UserDto
{
	public class CreateUserRequest
	{
		[Required]
		[MaxLength(50)]
		public string Username { get; set; }

		[Required]
		[MaxLength(255)]
		public string PasswordHash { get; set; }

		[Required]
		[MaxLength(255)]
		public string Email { get; set; }

	}
}
