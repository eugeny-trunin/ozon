namespace Ozon.Examination.Service.Models
{
    public class RateStatistics
    {
        /// <summary>
        /// Get or sets currency
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Get or sets max rate value
        /// </summary>
        public decimal Max { get; set; }

        /// <summary>
        /// Get or sets min rate value
        /// </summary>
        public decimal Min { get; set; }

        /// <summary>
        /// Get or sets median rate value
        /// </summary>
        public decimal Median { get; set; }
    }
}
