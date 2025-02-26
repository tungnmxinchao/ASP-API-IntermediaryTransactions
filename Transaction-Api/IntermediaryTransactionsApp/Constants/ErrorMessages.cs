namespace IntermediaryTransactionsApp.Constants
{
	public enum ErrorMessages
	{
		InvalidCredentials, 
		ObjectNotFound,
		ObjectNotFoundInToken,
		NotHavePermisson

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
				ErrorMessages.NotHavePermisson => "You do not have permission",
				_ => throw new NotImplementedException()
			};
		}
	}
}
