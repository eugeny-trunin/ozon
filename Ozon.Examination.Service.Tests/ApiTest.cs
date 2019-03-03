using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Ozon.Examination.Service.Persistence;
using Ozon.Examination.Service.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Tests
{
    public class ApiTest
    {
        [Test]
        public void ExchangeGet_LoadedRates_RatesStatistics()
        {
            var mockDataService = new Mock<IDataService>(MockBehavior.Strict);
            mockDataService
                .Setup(m => m.GetRateStatisticsAsync(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new[] { new Persistence.Entities.RateStatistics
                {
                    Currency = "USD",
                    From = new DateTime(2019, 1, 1),
                    To = new DateTime(2019, 1, 6),
                    Max = 22.634m,
                    Min = 22.494m,
                    Median = 22.594m,
                }});

            var client = new WebApplicationFactory<Startup>()
                 .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                 {
                     services.AddSingleton(mockDataService.Object);
                     services.Configure<ExchangeOptions>(option => option.Currencies = new[] { "USD" });
                 }))
                 .CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "api/exchange/2019/1");
            var response = client.SendAsync(request).Result;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var result = response.Content.ReadAsAsync<Models.RatesReport>().Result;
                Assert.NotNull(result);
                Assert.AreEqual(2019, result.Year);
                Assert.AreEqual(1, result.Month);
                Assert.AreEqual(1, result.WeekStatistics.First().From);
                Assert.AreEqual(6, result.WeekStatistics.First().To);
                Assert.AreEqual("USD", result.WeekStatistics.First().Rates.First().Currency);
                Assert.AreEqual(22.634m, result.WeekStatistics.First().Rates.First().Max);
                Assert.AreEqual(22.494m, result.WeekStatistics.First().Rates.First().Min);
                Assert.AreEqual(22.594m, result.WeekStatistics.First().Rates.First().Median);
                mockDataService.Verify(m => m.GetRateStatisticsAsync(
                    It.IsIn(2019),
                    It.IsIn<byte>(1),
                    It.Is<IEnumerable<string>>(arg => arg.Single() == "USD")),
                    Times.Once());
            });
        }

        [Test]
        public void ExchangeGet_NotLoadedRates_RatesStatisticsText()
        {
            var mockDataService = new Mock<IDataService>(MockBehavior.Strict);
            mockDataService
                .SetupSequence(m => m.GetRateStatisticsAsync(It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(Enumerable.Empty<Persistence.Entities.RateStatistics>())
                .ReturnsAsync(new[] { new Persistence.Entities.RateStatistics
                {
                    Currency = "IDR",
                    From = new DateTime(2018, 1, 2),
                    To = new DateTime(2018, 1, 4),
                    Max = 0.001576m,
                    Min = 0.001564m,
                    Median = 0.001571m,
                }});
            mockDataService
                .Setup(m => m.SaveRatesAsync(It.IsAny<IEnumerable<Persistence.Entities.Rate>>()))
                .Returns(Task.CompletedTask);

            var mockRateClient = new Mock<CzechNationalBank.IRateClient>(MockBehavior.Strict);
            mockRateClient
                .Setup(m => m.GetYearRatesAsync(It.IsAny<int>()))
                .ReturnsAsync(new[] { new CzechNationalBank.Rate
                {
                    Date = new DateTime(2018, 1, 1),
                    Currency = "IDR",
                    Amount = 1000,
                    Value = 1.578m,
                }});

            var client = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mockDataService.Object);
                    services.AddSingleton(mockRateClient.Object);
                    services.Configure<ExchangeOptions>(option => option.Currencies = new[] { "IDR" });
                }))
                .CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "api/exchange/2018/1");
            request.Headers.Add("Accept", "text/plain");
            var response = client.SendAsync(request).Result;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var result = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual("Year: 2018, month: January\n\nWeek periods:\n\n2...4: IDR - max:0.001576, min:0.001564, median:0.001571;", result);
                mockDataService.Verify(m => m.SaveRatesAsync(
                    It.Is<IEnumerable<Persistence.Entities.Rate>>(arg =>
                        arg.Any(r => r.Date == new DateTime(2018, 1, 1) && r.Currency == "IDR" && r.Value == 0.001578m))),
                    Times.Once());
            });
        }
    }
}
