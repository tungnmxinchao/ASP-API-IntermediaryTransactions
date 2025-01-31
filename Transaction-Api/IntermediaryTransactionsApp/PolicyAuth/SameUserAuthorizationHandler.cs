using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IntermediaryTransactionsApp.PolicyAuth
{
	public class SameUserAuthorizationHandler : AuthorizationHandler<SameUserRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement)
		{
			var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
			{
				context.Fail(); 
				return Task.CompletedTask;
			}

			if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
			{
				context.Succeed(requirement);
				return Task.CompletedTask;
			}

			if (context.Resource is Microsoft.AspNetCore.Http.HttpContext httpContext)
			{
				var routeId = httpContext.Request.RouteValues["id"]?.ToString();

				if (routeId == userId)
				{
					context.Succeed(requirement); 
				}
				else
				{
					context.Fail(); 
				}
			}
			else
			{
				context.Fail(); 
			}

			return Task.CompletedTask;
		}
	}

}
