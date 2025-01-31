using System.Net;
using System.Text.Json;

namespace IntermediaryTransactionsApp.Exceptions
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";

			var statusCode = exception switch
			{
				ObjectNotFoundException => (int)HttpStatusCode.NotFound,
				ValidationException => (int)HttpStatusCode.BadRequest,
				UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
				_ => (int)HttpStatusCode.InternalServerError
			};

			var response = new
			{
				Code = statusCode,
				Message = exception.Message,
				Details = statusCode == (int)HttpStatusCode.InternalServerError ? exception.StackTrace : null
			};

			context.Response.StatusCode = statusCode;

			return context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}
