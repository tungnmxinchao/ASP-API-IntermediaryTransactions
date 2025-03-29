using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.RoleDto;

namespace IntermediaryTransactionsApp.Dtos.UserDto
{
	public class GetUserResponse : CreateUserResponse
	{
        public int Id { get; set; }
        public RoleResponse role { get; set; }
	}
}
