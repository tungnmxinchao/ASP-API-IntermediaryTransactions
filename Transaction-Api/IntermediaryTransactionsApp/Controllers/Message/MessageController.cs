
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;


namespace IntermediaryTransactionsApp.Controllers.Message
{

    [Route("api/[controller]")]
    [Authorize(Policy = "CustomerPolicy")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var messages = await _messageService.GetMessages();

            if (messages == null || !messages.Any())
            {
                return NotFound(new ApiResponse<List<TransactionHistory>>(404, "No message found"));
            }

            return Ok(messages);
        }
    }
}
