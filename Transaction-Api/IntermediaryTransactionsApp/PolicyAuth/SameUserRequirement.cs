using Microsoft.AspNetCore.Authorization;

namespace IntermediaryTransactionsApp.PolicyAuth
{
	public class SameUserRequirement : IAuthorizationRequirement
	{
	}
}
