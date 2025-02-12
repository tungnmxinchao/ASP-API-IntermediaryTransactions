namespace IntermediaryTransactionsApp.Constants
{
	public enum ErrorMessages
	{
		InvalidCredentials, 
		ObjectNotFound,

	}

	public static class ErrorMessageExtensions
	{
		public static string GetMessage(this ErrorMessages error)
		{
			return error switch
			{
				ErrorMessages.InvalidCredentials => "Invalid username or password.",
				ErrorMessages.ObjectNotFound => "Object not found.",
				_ => throw new NotImplementedException()
			};
		}
	}
}
