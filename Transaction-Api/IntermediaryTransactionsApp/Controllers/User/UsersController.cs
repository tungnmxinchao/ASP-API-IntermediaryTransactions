using System.Net;
using System.Security.Claims;
using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Dtos.LoginDTO;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.Service;
using Microsoft.AspNetCore.Authorization;
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
		private readonly AuthService _authService;
		private readonly JwtService _jwtService;

		public UsersController(IMapper mapper, IUserService userService,
			AuthService authService, JwtService jwtService)
		{
			_mapper = mapper;
			_userService = userService;
			_authService = authService;
			_jwtService = jwtService;
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

        [Authorize(Policy = "SameUserPolicy")]
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, int Id)
        {
            var result = await _userService.UpdateUser(Id, request);


            if (!result)
            {
                ApiResponse<string> apiResponse = new ApiResponse<string>((int)HttpStatusCode.InternalServerError, "Update failed!");
                return BadRequest(apiResponse);
            }
            return Ok(result);

        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{Id}/admin-update-user")]
        public async Task<IActionResult> AdminUpdateUser([FromBody] UpdateUserRequest request, int Id)
        {
            var result = await _userService.UpdateUser(Id, request);


            if (!result)
            {
                ApiResponse<string> apiResponse = new ApiResponse<string>((int)HttpStatusCode.InternalServerError, "Update failed!");
                return BadRequest(apiResponse);
            }
            return Ok(result);

        }


        [HttpGet("{id}")]
		[Authorize(Policy = "SameUserPolicy")]
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

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var (accessToken, refreshToken) = await _authService.Login(request.Username, request.Password);

			var tokenResponse = new TokenResponse(accessToken, refreshToken);

			ApiResponse<TokenResponse> apiResponse = new ApiResponse<TokenResponse>(
				(int)HttpStatusCode.OK,
				"Login Successfully!",
				tokenResponse
			);

			return Ok(apiResponse);
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			var accessToken = await _jwtService.RefreshToken(request.UserId, request.RefreshToken);

			var response = new ApiResponse<string>(
			code: (int)HttpStatusCode.OK,
			message: "Token refreshed successfully.",
			data: accessToken);

			return Ok(response);
		}
		[HttpPost("revoke-token")]
		[Authorize]
		public async Task<IActionResult> RevokeToken()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized(new ApiResponse<string>((int)HttpStatusCode.Unauthorized, "Unauthorized. User ID not found."));
			}
			try
			{
				await _jwtService.RevokeRefreshToken(userId);

				return Ok(new ApiResponse<string>((int)HttpStatusCode.OK, "Token revoked successfully."));
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<string>(500, "An error occurred while revoking the token.", ex.Message));
			}
		}
	}
}
