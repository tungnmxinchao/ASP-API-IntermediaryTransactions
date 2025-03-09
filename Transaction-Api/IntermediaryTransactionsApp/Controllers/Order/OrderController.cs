
using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Interface.IOrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

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

        [HttpPut("{orderId}/complete")]
        public async Task<IActionResult> CompleteOrder(Guid orderId)
        {
            var result = await _orderService.CompleteOrder(orderId);
            if (result)
            {
                return Ok(new ApiResponse<UpdateOrderResponse>(200, "Complete order successfully"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to complete order"));
        }

        [HttpPut("{orderId}/complain")]
        public async Task<IActionResult> ComplainOrder(Guid orderId)
        {
            var result = await _orderService.ComplainOrder(orderId);
            if (result)
            {
                return Ok(new ApiResponse<UpdateOrderResponse>(200, "Complain order successfully"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to complain order"));
        }

        [HttpPut("{orderId}/requestBuyer-CheckOrder")]
        public async Task<IActionResult> RequestBuyerCheckOrder(Guid orderId)
        {
            var result = await _orderService.RequestBuyerCheckOrder(orderId);
            if (result)
            {
                return Ok(new ApiResponse<UpdateOrderResponse>(200, "Request buyer check order again successfully"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to request buyer check order again"));
        }

        [HttpPut("{orderId}/call-admin")]
        public async Task<IActionResult> CallAdmin(Guid orderId)
        {
            var result = await _orderService.CallAdmin(orderId);
            if (result)
            {
                return Ok(new ApiResponse<UpdateOrderResponse>(200, "Call admin handler order successfully"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to call admin handler order"));
        }

        [HttpPut("{orderId}/cancel-order")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var result = await _orderService.CancelOrder(orderId);
            if (result)
            {
                return Ok(new ApiResponse<UpdateOrderResponse>(200, "Cancel order successfully"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(500, "Failed to cancel order"));
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            var result = await _orderService.GetOrderDetail(orderId);

            if (result != null)
            {
                return Ok(new ApiResponse<OrderDetailResponse>(200, "Get order details successfully", result));
            }
            return StatusCode(StatusCodes.Status404NotFound, new ApiResponse<string>(404, "Not found order"));
        }

        [EnableQuery]
        [HttpGet]      
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetOrdersPublic();

            if (orders == null || !orders.Any())
            {
                return NotFound(new ApiResponse<List<OrdersPublicResponse>>(404, "No public orders found"));
            }

            return Ok(orders);

        }
    }
}
