﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IntermediaryTransactionsApp.Config;
using IntermediaryTransactionsApp.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IntermediaryTransactionsApp.Service
{
	public class JwtService
	{
		private readonly JwtSetting _jwtSettings;
		private readonly RedisService _redisService;
		private readonly ApplicationDbContext _dbContext;
		private readonly IHttpContextAccessor _contextAccessor;
		public JwtService(IOptions<JwtSetting> jwtSettings, RedisService redisService, 
			ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
		{
			_jwtSettings = jwtSettings.Value;
			_redisService = redisService;
			_dbContext = dbContext;
			_contextAccessor = contextAccessor;
		}

		public string GenerateAccessToken(IEnumerable<Claim> claims)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiration),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public string GenerateRefreshToken()
		{
			return Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");
		}

		public async Task<string> RefreshToken(string userId, string refreshToken)
		{

			var isValid = await ValidateRefreshToken(userId, refreshToken);
			if (!isValid)
			{
				throw new UnauthorizedAccessException("Invalid refresh token.");
			}

			var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id.ToString() == userId);
			if (user == null)
			{
				throw new UnauthorizedAccessException("User not found.");
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, user.Role.RoleName)

			};

			var newAccessToken = GenerateAccessToken(claims);

			return newAccessToken;
		}

		private async Task<bool> ValidateRefreshToken(string userId, string refreshToken)
		{
			var storedRefreshToken = await _redisService.GetTokenAsync($"refreshToken:{userId}");
			return storedRefreshToken == refreshToken;
		}

		public async Task RevokeRefreshToken(string userId)
		{
			var refreshTokenKey = $"refreshToken:{userId}";
			await _redisService.RevokeTokenAsync(refreshTokenKey);
		}

		public int? GetUserIdFromToken()
		{
			var claims = _contextAccessor.HttpContext?.User?.Claims;
			if (claims == null) return null;

			var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			return userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;
		}

	}
}
