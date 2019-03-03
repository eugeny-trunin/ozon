using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ozon.Examination.Service.CzechNationalBank;
using Ozon.Examination.Service.Models;
using Ozon.Examination.Service.Persistence;

namespace Ozon.Examination.Service.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly IRateClient rateClient;
        private readonly IDataService dataService;

        public ExchangeService(IRateClient rateClient, IDataService dataService)
        {
            this.rateClient = rateClient;
            this.dataService = dataService;
        }

        /// <inheritdoc />
        public async Task<RatesReport> GetRatesReportAsync(int year, byte month, IEnumerable<string> currencies)
        {
            if (currencies == null)
                throw new ArgumentNullException(nameof(currencies));

            var rateStatistics = await this.dataService.GetRateStatisticsAsync(year, month, currencies);
            if (!rateStatistics.Any())
            {
                var rates = await this.rateClient.GetYearRatesAsync(year);
                await this.dataService.SaveRatesAsync(rates.Select(r => ConvertRate(r)));

                rateStatistics = await this.dataService.GetRateStatisticsAsync(year, month, currencies);
            }

            return new RatesReport
            {
                Year = year,
                Month = month,
                WeekStatistics = rateStatistics
                    .GroupBy(r => new { from = r.From, to = r.To })
                    .Select(g => new WeekStatistics
                    {
                        From = (byte)g.Key.from.Day,
                        To = (byte)g.Key.to.Day,
                        Rates = g.Select(r => new RateStatistics
                        {
                            Currency = r.Currency,
                            Min = r.Min,
                            Max = r.Max,
                            Median = r.Median,
                        }).ToList(),
                    })
                    .OrderBy(r => r.From)
                    .ToArray()
            };
        }

        /// <inheritdoc />
        public async Task UpdateRatesAsync(DateTime date)
        {
            var rates = await this.rateClient.GetDailyRatesAsync(date.Date);

            await this.dataService.SaveRatesAsync(rates
                .Select(r => ConvertRate(r)));
        }

        private static Persistence.Entities.Rate ConvertRate(Service.CzechNationalBank.Rate rate)
        {
            return new Persistence.Entities.Rate
            {
                Currency = rate.Currency,
                Date = rate.Date,
                Value = rate.Value / rate.Amount,
            };
        }
    }
}
