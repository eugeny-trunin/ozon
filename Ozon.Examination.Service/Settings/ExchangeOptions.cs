using System.Collections.Generic;

namespace Ozon.Examination.Service.Settings
{
    public class ExchangeOptions
    {
        /// <summary>
        /// Get or sets currencies for report
        /// </summary>
        public IEnumerable<string> Currencies { get; set; }
    }
}
