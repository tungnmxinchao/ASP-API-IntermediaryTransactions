using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Interface.IOrderService;
using IntermediaryTransactionsApp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntermediaryTransactionsApp.Controllers.Order
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
		{

			bool result = await _orderService.CreateOrder(request);
			if (result)
			{
				return Ok(new ApiResponse<string>(200, "Order created successfully"));
			}

			return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to create order"));
		}
	}
}
