using System;

namespace Ozon.Examination.Service.Persistence.Entities
{
    public class RateStatistics
    {
        public string Currency { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public decimal Median { get; set; }
    }
}
