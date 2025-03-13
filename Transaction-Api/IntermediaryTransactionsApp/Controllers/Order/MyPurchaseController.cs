using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Dtos.OrderDto;
using IntermediaryTransactionsApp.Interface.IOrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace IntermediaryTransactionsApp.Controllers.Order
{
    [Route("api/[controller]")]
    [Authorize(Policy = "CustomerPolicy")]
    [ApiController]
    public class MyPurchaseController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public MyPurchaseController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetMyPurchaseOrders();

            if (orders == null || !orders.Any())
            {
                return NotFound(new ApiResponse<List<MyPurchase>>(404, "No order orders found"));
            }

            return Ok(orders);
        }
    }
}
