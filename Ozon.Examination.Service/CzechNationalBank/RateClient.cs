using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.CzechNationalBank
{
    public class RateClient : IRateClient
    {
        private readonly HttpClient httpClient;

        public RateClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<IList<Rate>> GetDailyRatesAsync(DateTime date)
        {
            using (var readStream = await this.GetResponse(FormattableString.Invariant($"daily.txt?date={date:dd.MM.yyyy}")))
                return await RatesParser.DailyParseAsync(readStream);
        }

        /// <inheritdoc />
        public async Task<IList<Rate>> GetYearRatesAsync(int year)
        {
            using (var readStream = await this.GetResponse(FormattableString.Invariant($"year.txt?year={year}")))
                return await RatesParser.YearParseAsync(readStream);
        }

        private async Task<StreamReader> GetResponse(string requestUri)
        {
            var response = await this.httpClient.GetAsync(requestUri);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException($"Czech National Bank return invalid status code [{response.StatusCode}]");

            if (!MediaTypeNames.Text.Plain.Equals(response.Content?.Headers?.ContentType?.MediaType, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"Czech National Bank return invalid content type [{response.Content?.Headers?.ContentType}]");

            return new StreamReader(
                await response.Content.ReadAsStreamAsync(),
                Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet ?? "UTF-8"));
        }
    }
}
