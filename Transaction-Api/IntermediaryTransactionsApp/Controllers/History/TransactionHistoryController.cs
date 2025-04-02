using System.Diagnostics;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.ApiDTO;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace IntermediaryTransactionsApp.Controllers.History
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public TransactionHistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [Authorize(Policy = "CustomerPolicy")]
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var histories = await _historyService.GetHistoryTransactions();

            if (histories == null || !histories.Any())
            {
                return NotFound(new ApiResponse<List<TransactionHistory>>(404, "No transactions found"));
            }

            return Ok(histories);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{Id}/{IsProcess}")]
        public async Task<IActionResult> UpdateHistory(int Id, bool IsProcess)
        {
            var result = await _historyService.UpdateHistory(Id, IsProcess);

            if (!result)
            {
                return BadRequest(new ApiResponse<List<string>>(404, "Update failed"));
            }

            return Ok(result);
        }
    }
}
