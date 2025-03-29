using AutoMapper;
using IntermediaryTransactionsApp.Constants;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Exceptions;
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

		public async Task<List<GetUserResponse>> FindAll()
		{
			var users = await _context.Users.Include(r => r.Role).ToListAsync();

			var usersResponse = _mapper.Map<List<GetUserResponse>>(users);

			return usersResponse;

        }

		public async Task<CreateUserResponse> CreateUser(CreateUserRequest createUserRequest)
		{
			if(createUserRequest != null)
			{
				var user = _mapper.Map<Users>(createUserRequest);

				user.IsActive = true;
				user.RoleId = 2;
				user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserRequest.PasswordHash);
				user.Money = 0;

				await _context.AddAsync(user);
				await _context.SaveChangesAsync();

				var userResponse = _mapper.Map<CreateUserResponse>(user);

				return userResponse;
			}
			throw new ValidationException("CreateUserRequest cannot be null.");
		}

		public async Task<GetUserResponse> GetUsersById(int id)
		{
			var user = await _context.Users.Include(r => r.Role)
				.FirstOrDefaultAsync(x => x.Id == id);

			if(user != null)
			{
				var userResponse = _mapper.Map<GetUserResponse>(user);
				return userResponse;
			}
			throw new ObjectNotFoundException($"User with ID {id} not found.");
		}

        public async Task<bool> UpdateMoney(UpdateMoneyRequest updateMoneyRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == updateMoneyRequest.UserId);

            if (user == null)
            {
                throw new ObjectNotFoundException($"User with ID {updateMoneyRequest.UserId} not found.");
            }

            switch (updateMoneyRequest.TypeUpdate)
            {
                case 1: 
                    user.Money += updateMoneyRequest.Money;
                    break;
                case 2: 
                    if (user.Money < updateMoneyRequest.Money)
                    {
                        throw new ValidationException("Your money not enough!");
                    }
                    user.Money -= updateMoneyRequest.Money;
                    break;
                default:
                    throw new ArgumentException("Invalid update money type.");
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

		public bool CheckBalanceUserWithMoney(decimal money, int userId)
		{
            var user =  _context.Users.Find(userId);

			if(user != null && user.Money < money)
			{
				return true;
			}
			return false;
        }

		public async Task<bool> UpdateUser(int userId, UpdateUserRequest request)
		{
            var user = _context.Users.Find(userId);

			if(user == null)
			{
                throw new ObjectNotFoundException(ErrorMessageExtensions.GetMessage(ErrorMessages.ObjectNotFound));
            }

			user.Email = request.Email;

            user.RoleId = request.RoleId.HasValue ? request.RoleId.Value : user.RoleId;
            user.IsActive = request.IsActive.HasValue ? request.IsActive.Value : user.IsActive;

			user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);

			return await _context.SaveChangesAsync() > 0;
        }

		
    }
}
