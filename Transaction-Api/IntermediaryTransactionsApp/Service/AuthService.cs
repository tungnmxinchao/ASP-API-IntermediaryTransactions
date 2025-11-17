using System;
using System.Security.Claims;
using IntermediaryTransactionsApp.Config;
using IntermediaryTransactionsApp.Constants;
using IntermediaryTransactionsApp.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace IntermediaryTransactionsApp.Service
{
    public class AuthService
    {
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _dbContext;
        private readonly RedisService _redisService;

        public AuthService(
            JwtService jwtService,
            ApplicationDbContext dbContext,
            RedisService redisService
        )
        {
            _jwtService = jwtService;
            _dbContext = dbContext;
            _redisService = redisService;
        }

        public async Task<(string AccessToken)> Login(
            string username,
            string password
        )
        {
            var user = await _dbContext
                .Users.Include(r => r.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    ErrorMessageExtensions.GetMessage(ErrorMessages.InvalidCredentials)
                );
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException(
                    ErrorMessageExtensions.GetMessage(ErrorMessages.InvalidCredentials)
                );
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
            };

            var accessToken = _jwtService.GenerateAccessToken(claims);
            //var refreshToken = _jwtService.GenerateRefreshToken();

            //var refreshTokenKey = $"refreshToken:{user.Id}";
            //var refreshTokenExpiry = TimeSpan.FromMinutes(JwtSetting.RefreshTokenExpiration);

            //await _redisService.StoreTokenAsync(refreshTokenKey, refreshToken, refreshTokenExpiry);

            return (accessToken);
        }
    }
}
