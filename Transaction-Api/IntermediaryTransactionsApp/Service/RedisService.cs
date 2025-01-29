using StackExchange.Redis;

namespace IntermediaryTransactionsApp.Service
{
	public class RedisService
	{
		private readonly IConnectionMultiplexer _redis;

		public RedisService(IConnectionMultiplexer redis)
		{
			_redis = redis;
		}

		public async Task StoreTokenAsync(string key, string token, TimeSpan expiration)
		{
			var db = _redis.GetDatabase();
			await db.StringSetAsync(key, token, expiration);
		}

		public async Task<string?> GetTokenAsync(string key)
		{
			var db = _redis.GetDatabase();
			var token = await db.StringGetAsync(key);

			return token.HasValue ? token.ToString() : null;
		}

		public async Task<bool> ValidateTokenAsync(string key, string token)
		{
			var db = _redis.GetDatabase();
			var storedToken = await db.StringGetAsync(key);
			return storedToken == token;
		}

		public async Task RevokeTokenAsync(string key)
		{
			var db = _redis.GetDatabase();
			await db.KeyDeleteAsync(key);
		}


	}
}
