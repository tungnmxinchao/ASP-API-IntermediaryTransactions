using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.UserDto;

namespace IntermediaryTransactionsApp.Interface.UserInterface
{
	public interface IUserService
	{
		public Task<CreateUserResponse> CreateUser(CreateUserRequest createUserRequest);

		public Task<GetUserResponse> GetUsersById(int id);
	}
}
