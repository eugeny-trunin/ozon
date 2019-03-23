using Ozon.Examination.Service.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Services
{
    public interface IExchangeService
    {
        /// <summary>
        /// Get retes statistics report for month
        /// </summary>
        /// <param name="year">Year report</param>
        /// <param name="month">Month report</param>
        /// <param name="currencies">List currency</param>
        /// <returns>Rates report</returns>
        Task<RatesReport> GetRatesReportAsync(int year, byte month, IEnumerable<string> currencies);

        /// <summary>
        /// Get retes statistics report for year
        /// </summary>
        /// <param name="year">Year report</param>
        /// <param name="currencies">List currency</param>
        /// <returns>List month rates report</returns>
        Task<IEnumerable<RatesReport>> GetRatesReportAsync(int year, IEnumerable<string> currencies);

        /// <summary>
        /// Update rates by date
        /// </summary>
        /// <param name="date">Date</param>
        Task UpdateRatesAsync(DateTime date);
    }
}
