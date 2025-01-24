using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
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
		public IActionResult CreateUser([FromBody] CreateUserRequest createUserRequest)
		{
			var user = _userService.CreateUser(createUserRequest);
			if (user != null)
			{
				return CreatedAtAction(nameof(CreateUser), user);
			}
			return BadRequest();

		}
		[HttpGet("{id}")]
		public IActionResult GetUserById([FromRoute] int id)
		{
			var user = _userService.GetUsersById(id);

			if(user != null)
			{
				return Ok(user);
			}
			return BadRequest();
		}
	}
}
