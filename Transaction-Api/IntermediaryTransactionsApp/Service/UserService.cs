using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Interface.UserInterface;
using Microsoft.EntityFrameworkCore;

namespace IntermediaryTransactionsApp.Service
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public UserService(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public CreateUserResponse CreateUser(CreateUserRequest createUserRequest)
		{
			if(createUserRequest != null)
			{
				var user = _mapper.Map<Users>(createUserRequest);

				user.IsActive = true;
				user.RoleId = 1;
				user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserRequest.PasswordHash);

				_context.Add(user);
				_context.SaveChanges();

				var userResponse = _mapper.Map<CreateUserResponse>(user);

				return userResponse;
			}
			throw new Exception("User not found!");
		}

		public GetUserResponse GetUsersById(int id)
		{
			var user = _context.Users.Include(r => r.Role)
				.FirstOrDefault(x => x.Id == id);

			if(user != null)
			{
				var userResponse = _mapper.Map<GetUserResponse>(user);
				return userResponse;
			}
			throw new Exception("Id of user not found");
		}
	}
}
