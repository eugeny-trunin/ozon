using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ozon.Examination.Service.Models
{
    public class RatesReport
    {
        /// <summary>
        /// Get or sets year of report
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Get or sets month of report
        /// </summary>
        public byte Month { get; set; }

        /// <summary>
        /// Get or sets weekly statistics
        /// </summary>
        public IEnumerable<WeekStatistics> WeekStatistics { get; set; }

        public override string ToString()
        {
            return
                FormattableString.Invariant($"Year: {Year}, month: {new DateTimeFormatInfo().GetMonthName(Month)}\n\n")
                + "Week periods:"
                + WeekStatistics.Aggregate(string.Empty, (s1, ws) => s1 + FormattableString.Invariant($"\n\n{ws.From}...{ws.To}:")
                    + ws.Rates.Aggregate(string.Empty, (s2, r) => s2 + FormattableString.Invariant($" {r.Currency} - max:{r.Max}, min:{r.Min}, median:{r.Median};")));
        }
    }
}
