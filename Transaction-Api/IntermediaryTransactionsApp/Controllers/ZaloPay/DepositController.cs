using IntermediaryTransactionsApp.Config;
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
            return Ok(result);
        }
    }

    public class DepositRequest
    {
        public int UserId { get; set; }
        public int Amount { get; set; }
    }
}
