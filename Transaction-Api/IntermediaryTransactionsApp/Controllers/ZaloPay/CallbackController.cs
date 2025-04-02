using IntermediaryTransactionsApp.Utils.Crypto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IntermediaryTransactionsApp.Controllers.ZaloPay
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private string key2 = "trMrHtvjo6myautxDUiAcYsVtaeQ8nhf";

        [HttpPost]
        public IActionResult Post([FromBody] dynamic cbdata)
        {
            var result = new Dictionary<string, object>();

            try
            {
                var dataStr = Convert.ToString(cbdata["data"]);
                var reqMac = Convert.ToString(cbdata["mac"]);
                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);
                Console.WriteLine("mac = {0}", mac);

                if (!reqMac.Equals(mac))
                {
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                }
                else
                {
                    var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                    Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);
                    result["return_code"] = 1;
                    result["return_message"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["return_code"] = 0;
                result["return_message"] = ex.Message;
            }

            return Ok(result);
        }
    }
}
