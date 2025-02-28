using FinancialApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; 


namespace FinancialApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly FinancialApiService _financialApiService;

        public ReportsController(FinancialApiService financialApiService)
        {
            _financialApiService = financialApiService;
        }

        [HttpGet("stocks/{symbol}/history")]
        public async Task<IActionResult> GetStockHistory(string symbol)
        {
            var stockHistory = await _financialApiService.GetStockHistory(symbol);
            if (stockHistory == null)
            {
                return NotFound();
            }
            return Ok(stockHistory);
        }

        [HttpGet("crypto/{symbol}/weekly")]
        public async Task<IActionResult> GetCryptoWeekly(string symbol)
        {
            var cryptoWeekly = await _financialApiService.GetCryptoWeekly(symbol);
            if (cryptoWeekly == null)
            {
                return NotFound();
            }
            return Ok(cryptoWeekly);
        }
   }
}
