﻿namespace IntermediaryTransactionsApp.Dtos.LoginDTO
{
	public class RefreshTokenRequest
	{
		public string UserId { get; set; }
		public string RefreshToken { get; set; }
	}
}
