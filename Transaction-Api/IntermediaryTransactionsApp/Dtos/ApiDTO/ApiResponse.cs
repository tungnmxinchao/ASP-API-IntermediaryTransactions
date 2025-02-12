

using System.Text.Json.Serialization;

namespace IntermediaryTransactionsApp.Dtos.ApiDTO
{

	public class ApiResponse<T>
	{
		public int Code {  get; set; }
		public string Message { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public T? Data { get; set; }

		public ApiResponse(int code, string message, T data)
		{
			this.Code = code;
			this.Message = message;
			this.Data = data;
		}

		public ApiResponse(int code, string message)
		{
			this.Code = code;
			this.Message = message;
		}
	}
}
