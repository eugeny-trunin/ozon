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

            return (await GetRatesReportAsync(
                new DateTime(year, month, 1),
                new DateTime(year, month, 1).AddMonths(1).AddDays(-1),
                currencies)).SingleOrDefault();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RatesReport>> GetRatesReportAsync(int year, IEnumerable<string> currencies)
        {
            if (currencies == null)
                throw new ArgumentNullException(nameof(currencies));

            return (await GetRatesReportAsync(
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                currencies)).ToArray();
        }

        private async Task<IEnumerable<RatesReport>> GetRatesReportAsync(DateTime from, DateTime to, IEnumerable<string> currencies)
        {
            var rateStatistics = await this.dataService.GetRateStatisticsAsync(from, to, currencies);
            if (!rateStatistics.Any())
            {
                foreach(var year in Enumerable.Range(from.Year, from.Year-to.Year+1))
                {
                    var rates = await this.rateClient.GetYearRatesAsync(year);
                    await this.dataService.SaveRatesAsync(rates.Select(r => ConvertRate(r)));
                }

                rateStatistics = await this.dataService.GetRateStatisticsAsync(from, to, currencies);
            }

            return rateStatistics
                .GroupBy(rs => new { year = rs.From.Year, month = rs.From.Month })
                .Select(gm => new RatesReport
                {
                    Year = gm.Key.year,
                    Month = (byte)gm.Key.month,
                    WeekStatistics = gm
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
                })
                .OrderBy(rr => rr.Year * 12 + rr.Month);
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
