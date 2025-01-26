﻿namespace IntermediaryTransactionsApp.Dtos.LoginDTO
{
	public class TokenResponse
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }

		public TokenResponse(string accessToken, string refreshToken)
		{
			AccessToken = accessToken;
			RefreshToken = refreshToken;
		}
	}
}
