using IntermediaryTransactionsApp.Db.Models;

namespace IntermediaryTransactionsApp.Dtos.UserDto
{
	public class GetUserResponse : CreateUserResponse
	{
		public string RoleName { get; set; }
		public string RoleDescription { get; set; }
	}
}
