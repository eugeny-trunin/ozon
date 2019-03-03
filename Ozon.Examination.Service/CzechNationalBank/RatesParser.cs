using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.CzechNationalBank
{
    public static class RatesParser
    {
        private const char FieldSeparator = '|';
        private const string DateFormat = "dd.MMM yyyy";
        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo("en");

        private const int DayAmountIndex = 2;
        private const int DayCurrencyIndex = 3;
        private const int DayRateIndex = 4;

        private static readonly Regex YearHeaderRegex = new Regex("(\\d+) (\\w{3})", RegexOptions.Singleline);

        /// <summary>
        /// Parse daily rates report
        /// </summary>
        /// <param name="streamReader">Stream reader</param>
        /// <returns>List of rate data</returns>
        public static async Task<IList<Rate>> DailyParseAsync(StreamReader streamReader)
        {
            if (streamReader == null)
                throw new ArgumentNullException(nameof(streamReader));

            var rates = new List<Rate>();

            var row = await streamReader.ReadLineAsync();
            var date = DateTime.ParseExact(row.Substring(0, row.IndexOf(" #")), DateFormat, CultureInfo);
            row = await streamReader.ReadLineAsync();

            while (!streamReader.EndOfStream)
            {
                var columns = (await streamReader.ReadLineAsync()).Split(FieldSeparator);
                rates.Add(new Rate
                    {
                        Date = date,
                        Currency = columns[DayCurrencyIndex],
                        Amount = int.Parse(columns[DayAmountIndex], CultureInfo),
                        Value = decimal.Parse(columns[DayRateIndex], CultureInfo),
                    });
            }

            return await Task.FromResult(rates);
        }

        /// <summary>
        /// Parse year rates report
        /// </summary>
        /// <param name="streamReader">Stream reader</param>
        /// <returns>List of rate data</returns>
        public static async Task<IList<Rate>> YearParseAsync(StreamReader streamReader)
        {
            if (streamReader == null)
                throw new ArgumentNullException(nameof(streamReader));

            var rates = new List<Rate>();

            var headers = (await streamReader.ReadLineAsync())
                .Split(FieldSeparator)
                .Skip(1)
                .Select(s =>
                {
                    var match = YearHeaderRegex.Match(s);
                    return new
                    {
                        amount = int.Parse(match.Groups[1].Value, CultureInfo),
                        currency = match.Groups[2].Value,
                    };
                }).ToArray();

            while (!streamReader.EndOfStream)
            {
                var row = (await streamReader.ReadLineAsync()).Split(FieldSeparator);
                var date = DateTime.ParseExact(row.First(), DateFormat, CultureInfo);
                rates.AddRange(row
                    .Skip(1)
                    .Select((rate, i) => new Rate
                    {
                        Date = date,
                        Currency = headers[i].currency,
                        Amount = headers[i].amount,
                        Value = decimal.Parse(rate, CultureInfo),
                    }));
            }

            return await Task.FromResult(rates);
        }
    }
}
