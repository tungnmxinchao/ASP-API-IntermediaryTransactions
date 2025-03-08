using System.Net;
using System.Text.Json;
using IntermediaryTransactionsApp.Dtos.ApiDTO;

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
                InvalidOperationException => (int)HttpStatusCode.InternalServerError,
                _ => (int)HttpStatusCode.InternalServerError
			};



			var response = new ApiResponse<string>(statusCode, exception.Message);

			context.Response.StatusCode = statusCode;

			return context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}
