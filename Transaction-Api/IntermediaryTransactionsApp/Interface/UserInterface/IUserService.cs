using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.UserDto;

namespace IntermediaryTransactionsApp.Interface.UserInterface
{
	public interface IUserService
	{
		public CreateUserResponse CreateUser(CreateUserRequest createUserRequest);

		public GetUserResponse GetUsersById(int id);
	}
}
