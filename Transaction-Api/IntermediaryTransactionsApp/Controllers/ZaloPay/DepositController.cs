using IntermediaryTransactionsApp.Config;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.HistoryDto;
using IntermediaryTransactionsApp.Dtos.MessageDto;
using IntermediaryTransactionsApp.Dtos.UserDto;
using IntermediaryTransactionsApp.Enum;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.Service;
using IntermediaryTransactionsApp.Utils;
using IntermediaryTransactionsApp.Utils.Crypto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IntermediaryTransactionsApp.Controllers.ZaloPay
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private static string app_id = "2554";
        private static string key1 = "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
        private static string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        private readonly IHistoryService _historyService;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public DepositController(IHistoryService historyService, 
            ApplicationDbContext context, IUserService userService)
        {
            _historyService = historyService;
            _context = context;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            Random rnd = new Random();
            var app_trans_id = DateTime.Now.ToString("yyMMdd") + "_" + rnd.Next(1000000);
            var param = new Dictionary<string, string>();

            param.Add("app_id", app_id);
            param.Add("app_user", request.UserId.ToString());
            param.Add("app_time", GetData.GetTimeStamp().ToString());
            param.Add("amount", request.Amount.ToString());
            param.Add("app_trans_id", app_trans_id);
            param.Add("embed_data", JsonConvert.SerializeObject(new { redirecturl = ZaloConfig.RedirectUrl }));
            param.Add("item", JsonConvert.SerializeObject(new[] { new { } }));
            param.Add("description", "Nạp tiền vào ví " + request.UserId);
            param.Add("bank_code", "zalopayapp");
            param.Add("callback_url", ZaloConfig.CallbackUrl);

            var data = string.Join("|", app_id, param["app_trans_id"], param["app_user"], param["amount"], param["app_time"], param["embed_data"], param["item"]);
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(create_order_url, param);

            await RecordHistory(request.UserId, request.Amount);

            await UpdateUserBalance(request.UserId, request.Amount);

            _context.SaveChanges();


            var response = new
            {
                result = result,
                app_trans_id = app_trans_id
            };

            return Ok(response);
        }

        private async Task RecordHistory(int userId, int amount)
        {
            var historyRequest = new CreateHistoryRequest
            {
                Amount = amount,
                TransactionType = (int)UpdateMoneyMode.AddMoney,
                Note = $"Nạp tiền vào hệ thống cho người dùng: {userId}",
                Payload = "Giao dịch thành công",
                UserId = userId,
                OnDoneLink = "None"
            };

            await _historyService.CreateHistory(historyRequest);          
        }

        private async Task UpdateUserBalance(int userId, int amount)
        {
            var updateMoney = new UpdateMoneyRequest
            {
                UserId = userId,
                Money = amount,
                TypeUpdate = (int)UpdateMoneyMode.AddMoney
            };

            await _userService.UpdateMoney(updateMoney);
        }




    }

    public class DepositRequest
    {
        public int UserId { get; set; }
        public int Amount { get; set; }
    }
}
