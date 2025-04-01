using IntermediaryTransactionsApp.Utils.Crypto;
using IntermediaryTransactionsApp.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntermediaryTransactionsApp.Controllers.ZaloPay
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryOrderController : ControllerBase
    {
        private static string app_id = "2554";
        private static string key1 = "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
        private static string query_order_url = "https://sb-openapi.zalopay.vn/v2/query";

        [HttpGet("{appTransId}")]
        public async Task<IActionResult> GetOrderStatus(string appTransId)
        {
            var param = new Dictionary<string, string>
            {
                { "app_id", app_id },
                { "app_trans_id", appTransId }
            };

            var data = string.Join("|", app_id, appTransId, key1);
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(query_order_url, param);
            return Ok(result);
        }
    }
}
