using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using IntermediaryTransactionsApp.Interface.IOrderService;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using IntermediaryTransactionsApp.Interface.UserInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace IntermediaryTransactionsApp.Controllers.Admin
{
    [Authorize(Policy = "AdminPolicy")]
    [ApiController]
    public class AdminController : ODataController
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IHistoryService _historyService;
        private readonly IMessageService _messageService;

        public AdminController(IUserService userService, 
            IOrderService orderService, IHistoryService 
            historyService, IMessageService messageService)
        {
            _userService = userService;
            _orderService = orderService;
            _historyService = historyService;
            _messageService = messageService;
        }

        [HttpGet("odata/Users")]
        [EnableQuery]
        public async Task<IActionResult> FindAllUsers()
        {
            var users = await _userService.FindAll();

            return Ok(users);

        }

        [HttpGet("odata/AdminViewOrders")]
        [EnableQuery]
        public async Task<IActionResult> FindAllOrders()
        {
            var orders = await _orderService.FindAll();

            return Ok(orders);

        }

        [EnableQuery]
        [HttpGet("odata/AdminViewTransactions")]
        public async Task<IActionResult> FindAllTransactions()
        {
            var histories = await _historyService.FindAll();

            if (histories == null || !histories.Any())
            {
                return NotFound(new ApiResponse<List<AdminTransactionHistory>>(404, "No transactions found"));
            }

            return Ok(histories);
        }
    }
}
