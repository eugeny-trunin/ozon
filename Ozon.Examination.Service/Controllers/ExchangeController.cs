using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ozon.Examination.Service.Models;
using Ozon.Examination.Service.Services;
using Ozon.Examination.Service.Settings;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeService exchangeService;
        private readonly ExchangeOptions options;

        public ExchangeController(IExchangeService exchangeService, IOptions<ExchangeOptions> options)
        {
            this.exchangeService = exchangeService;
            this.options = options.Value;
        }

        [HttpGet("{year:int}/{month:int}")]
        public async Task<IActionResult> Get([FromRoute] RetesReportRequest request)
        {
            return Ok(await this.exchangeService.GetRatesReportAsync(request.Year, request.Month, this.options.Currencies));
        }
    }
}
