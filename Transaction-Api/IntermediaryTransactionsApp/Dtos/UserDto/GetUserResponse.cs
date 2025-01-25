using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.RoleDto;

namespace IntermediaryTransactionsApp.Dtos.UserDto
{
	public class GetUserResponse : CreateUserResponse
	{
		public RoleResponse role { get; set; }
	}
}
