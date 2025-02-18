namespace IntermediaryTransactionsApp.Constants
{
	public enum ErrorMessages
	{
		InvalidCredentials, 
		ObjectNotFound,
		ObjectNotFoundInToken

	}

	public static class ErrorMessageExtensions
	{
		public static string GetMessage(this ErrorMessages error)
		{
			return error switch
			{
				ErrorMessages.InvalidCredentials => "Invalid username or password.",
				ErrorMessages.ObjectNotFound => "Object not found.",
				ErrorMessages.ObjectNotFoundInToken => "Object not found in token",
				_ => throw new NotImplementedException()
			};
		}
	}
}
