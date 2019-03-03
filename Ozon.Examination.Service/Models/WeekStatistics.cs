using System.Collections.Generic;

namespace Ozon.Examination.Service.Models
{
    public class WeekStatistics
    {
        /// <summary>
        /// Get or sets from day
        /// </summary>
        public byte From { get; set; }

        /// <summary>
        /// Get or sets to day
        /// </summary>
        public byte To { get; set; }

        /// <summary>
        /// Get or sets rates statistics
        /// </summary>
        public IList<RateStatistics> Rates { get; set; }
    }
}
