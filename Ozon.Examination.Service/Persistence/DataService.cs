using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using Ozon.Examination.Service.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Persistence
{
    public class DataService : IDataService
    {
        private readonly RateDbContext rateDbContext;

        public DataService(RateDbContext rateDbContext)
        {
            this.rateDbContext = rateDbContext;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RateStatistics>> GetRateStatisticsAsync(DateTime from, DateTime to, IEnumerable<string> currencies)
        {
            if (currencies == null)
                throw new ArgumentNullException(nameof(currencies));
            if (from.Date > to.Date)
                throw new ArgumentException($"Invalid range {from.Date}-{to.Date}");

            return await this.rateDbContext.RateStatistics.FromSql(
                @"SELECT Min(""Date"") AS ""From"", Max(""Date"") AS ""To"", ""Currency"",
Min(""Value"") AS ""Min"", Max(""Value"") AS ""Max"",
percentile_disc(0.5) within group(order by ""Value"") AS ""Median""
FROM public.""Rate""
WHERE ""Date"" BETWEEN @from AND @to AND ""Currency"" = ANY(@currencies)
GROUP BY EXTRACT(YEAR FROM ""Date""), EXTRACT(MONTH FROM ""Date""), EXTRACT(WEEK FROM ""Date""), ""Currency""",
                new NpgsqlParameter("from", NpgsqlDbType.Date) { Value = from.Date },
                new NpgsqlParameter("to", NpgsqlDbType.Date) { Value = to.Date },
                new NpgsqlParameter("currencies", NpgsqlDbType.Array | NpgsqlDbType.Char) { Value = currencies })
                    .AsNoTracking()
                    .ToListAsync();
        }

        /// <inheritdoc />
        public async Task SaveRatesAsync(IEnumerable<Rate> rates)
        {
            if (rates == null)
                throw new ArgumentNullException(nameof(rates));

            await this.rateDbContext.Rates.AddRangeAsync(rates
                .Where(r => !this.rateDbContext.Rates.Any(rs => rs.Date == r.Date && rs.Currency == r.Currency)));
            await this.rateDbContext.SaveChangesAsync();
        }
    }
}
