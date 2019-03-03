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
        public async Task<IEnumerable<RateStatistics>> GetRateStatisticsAsync(int year, byte month, IEnumerable<string> currencies)
        {
            if (currencies == null)
                throw new ArgumentNullException(nameof(currencies));

            return await this.rateDbContext.RateStatistics.FromSql(
                @"SELECT Min(""Date"") AS ""From"", Max(""Date"") AS ""To"", ""Currency"",
Min(""Value"") AS ""Min"", Max(""Value"") AS ""Max"",
percentile_disc(0.5) within group(order by ""Value"") AS ""Median""
FROM public.""Rate""
WHERE ""Date"" BETWEEN @from AND @to AND ""Currency"" = ANY(@currencies)
GROUP BY EXTRACT(WEEK FROM ""Date""), ""Currency""",
                new NpgsqlParameter("from", NpgsqlDbType.Date) { Value = new DateTime(year, month, 1) },
                new NpgsqlParameter("to", NpgsqlDbType.Date) { Value = new DateTime(year, month, 1).AddMonths(1).AddDays(-1) },
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
