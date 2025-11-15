using System.Text.Json;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Utils.Crypto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IntermediaryTransactionsApp.Controllers.ZaloPay
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private string key2 = "trMrHtvjo6myautxDUiAcYsVtaeQ8nhf";

        private readonly ApplicationDbContext _context;

        public CallbackController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement cbdata)
        {
            var result = new Dictionary<string, object>();

            try
            {
                // lấy data và mac
                string dataStr = cbdata.GetProperty("data").GetString();
                string reqMac = cbdata.GetProperty("mac").GetString();

                // tính lại mac
                string mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                if (!reqMac.Equals(mac))
                {
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                    return Ok(result);
                }

                // parse JSON bên trong data
                var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);

                string appTransId = dataJson["app_trans_id"].ToString();
                int amount = Convert.ToInt32(dataJson["amount"]);
                int userId = Convert.ToInt32(dataJson["app_user"]);

                // query và update TransactionHistory + User.Money
                var history = await _context.TransactionHistories.FirstOrDefaultAsync(h =>
                    h.Payload == appTransId
                );

                if (history != null)
                {
                    history.IsProcessed = true;
                    history.UpdatedAt = DateTime.Now;
                    _context.TransactionHistories.Update(history);
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    user.Money += amount;
                    user.UpdatedAt = DateTime.Now;
                    _context.Users.Update(user);
                }

                await _context.SaveChangesAsync();

                result["return_code"] = 1;
                result["return_message"] = "success";
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
