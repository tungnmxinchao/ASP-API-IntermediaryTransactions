namespace IntermediaryTransactionsApp.Constants
{
	public enum ErrorMessages
	{
		InvalidCredentials, 
		ObjectNotFound,
		ObjectNotFoundInToken,
		NotHavePermisson,
		BalanceNotEnough

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
                ErrorMessages.BalanceNotEnough => "You do not have enough money",
                _ => throw new NotImplementedException()
			};
		}
	}
}
