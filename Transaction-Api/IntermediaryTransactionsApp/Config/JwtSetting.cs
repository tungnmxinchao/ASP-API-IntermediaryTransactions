namespace IntermediaryTransactionsApp.Config
{
	public class JwtSetting
	{
		public string SecretKey { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public int AccessTokenExpiration { get; set; }
		public static int RefreshTokenExpiration { get; set; }
	}
}
