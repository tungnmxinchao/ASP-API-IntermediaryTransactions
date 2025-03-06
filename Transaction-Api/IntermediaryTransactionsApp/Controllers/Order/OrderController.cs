using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Interface.IOrderService;
using IntermediaryTransactionsApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntermediaryTransactionsApp.Controllers.Order
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "CustomerPolicy")]
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
                return Created($"/orders/", new ApiResponse<string>(201, "Order created successfully"));
            }

			return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to create order"));
		}
		[HttpPut]
		public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest request)
		{
			var order = await _orderService.UpdateOrder(request);
			if (order != null)
			{
				return Ok(new ApiResponse<UpdateOrderResponse>(200, "Order updated successfully", order));
			}
			return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to create order"));

		}

		[HttpPut("buy-order")]
		public async Task<IActionResult> BuyOrder([FromBody] BuyOrderRequest request)
		{
			var buyOrder = await _orderService.BuyOrder(request);

			if (buyOrder)
			{
                return Ok(new ApiResponse<UpdateOrderResponse>(200, "Buy order successfully"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to buy order"));
        }
	}
}
