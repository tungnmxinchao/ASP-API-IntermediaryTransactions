﻿using AutoMapper;
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
				throw new ObjectNotFoundException($"User with ID {user.Id} not found.");
			}

			if (user.Money < updateMoneyRequest.Money)
			{
				throw new ValidationException("Your money not enought!");
			}			

			user.Money -= updateMoneyRequest.Money;

			_context.Users.Update(user);
			int rowAffected = await _context.SaveChangesAsync();

			return rowAffected > 0;
		}
	}
}
