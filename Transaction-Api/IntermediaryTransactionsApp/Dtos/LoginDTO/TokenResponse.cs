namespace IntermediaryTransactionsApp.Dtos.LoginDTO
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public TokenResponse(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}
