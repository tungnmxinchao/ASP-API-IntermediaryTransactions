using System.Net;
using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Interface.UserInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntermediaryTransactionsApp.Controllers.User
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IUserService _userService;

		public UsersController(IMapper mapper, IUserService userService)
		{
			_mapper = mapper;
			_userService = userService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
		{
			var user = await _userService.CreateUser(createUserRequest);

			
			if (user != null)
			{
				ApiResponse<CreateUserResponse> apiResponse = new ApiResponse<CreateUserResponse>((int) HttpStatusCode.Created, "Add Successfully!", user);
				return CreatedAtAction(nameof(CreateUser), apiResponse);
			}
			return BadRequest();

		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById([FromRoute] int id)
		{
			var user = await _userService.GetUsersById(id);

			if(user != null)
			{
				ApiResponse<GetUserResponse> apiResponse = new ApiResponse<GetUserResponse>((int)HttpStatusCode.OK, "Get Data Successfully!", user);
				return Ok(apiResponse);
			}
			return BadRequest();
		}
	}
}
