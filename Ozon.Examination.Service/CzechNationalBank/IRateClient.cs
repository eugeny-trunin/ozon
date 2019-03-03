using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.CzechNationalBank
{
    public interface IRateClient
    {
        /// <summary>
        /// Get daily rates
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>List of rate data</returns>
        Task<IList<Rate>> GetDailyRatesAsync(DateTime date);

        /// <summary>
        /// Get year rates
        /// </summary>
        /// <param name="year">Year</param>
        /// <returns>List of rate data</returns>
        Task<IList<Rate>> GetYearRatesAsync(int year);
    }
}
