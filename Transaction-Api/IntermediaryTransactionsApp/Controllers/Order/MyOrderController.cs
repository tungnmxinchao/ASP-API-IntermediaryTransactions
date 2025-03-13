using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Interface.IOrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace IntermediaryTransactionsApp.Controllers.Order
{
    [Route("api/[controller]")]
    [Authorize(Policy = "CustomerPolicy")]
    [ApiController]
    public class MyOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public MyOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetMySaleOrders();

            if (orders == null || !orders.Any())
            {
                return NotFound(new ApiResponse<List<IntermediaryTransactionsApp.Db.Models.Order>>(404, "No order orders found"));
            }

            return Ok(orders);
        }
    }
}
