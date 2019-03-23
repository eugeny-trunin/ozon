using Ozon.Examination.Service.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Persistence
{
    public interface IDataService
    {
        /// <summary>
        /// Save rates
        /// </summary>
        /// <param name="rates">Rates</param>
        Task SaveRatesAsync(IEnumerable<Rate> rates);

        /// <summary>
        /// Get rate statistics
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <param name="currencies">List currencies</param>
        /// <returns>List rate statistics</returns>
        Task<IEnumerable<RateStatistics>> GetRateStatisticsAsync(DateTime from, DateTime to, IEnumerable<string> currencies);
    }
}
