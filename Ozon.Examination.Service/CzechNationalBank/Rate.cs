using System;

namespace Ozon.Examination.Service.CzechNationalBank
{
    public class Rate
    {
        /// <summary>
        /// Gets or sets date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets currency code (ISO 4217)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets amount
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets rate
        /// </summary>
        public decimal Value { get; set; }
    }
}
