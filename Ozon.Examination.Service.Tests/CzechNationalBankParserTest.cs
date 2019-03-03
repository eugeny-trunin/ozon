using NUnit.Framework;
using Ozon.Examination.Service.CzechNationalBank;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ozon.Examination.Service.Tests
{
    public class CzechNationalBankParserTest
    {
        [Test]
        public void DailyParse_DayTextData_ParsedRates()
        {
            using (var sr = GetTestData("day-response.txt"))
            {
                var rates = RatesParser.DailyParseAsync(sr).Result;
                Assert.Multiple(() => {
                    Assert.AreEqual(33, rates.Count);
                    var rateUsd = rates.Single(r => r.Currency == "USD");
                    Assert.AreEqual(new DateTime(2019, 2, 22), rateUsd.Date);
                    Assert.AreEqual(1, rateUsd.Amount);
                    Assert.AreEqual(22.662m, rateUsd.Value);
                    var rateIdr = rates.Single(r => r.Currency == "IDR");
                    Assert.AreEqual(new DateTime(2019, 2, 22), rateIdr.Date);
                    Assert.AreEqual(1000, rateIdr.Amount);
                    Assert.AreEqual(1.612m, rateIdr.Value);
                });
            }
        }

        [Test]
        public void YearParse_YearTextData_ParsedRates()
        {
            using (var sr = GetTestData("year-response.txt"))
            {
                var rates = RatesParser.YearParseAsync(sr).Result;
                Assert.Multiple(() => {
                    Assert.AreEqual(33 * 38, rates.Count);
                    var rateUsd = rates.Single(r => r.Date == new DateTime(2019, 1, 25) && r.Currency == "USD");
                    Assert.AreEqual(1, rateUsd.Amount);
                    Assert.AreEqual(22.644m, rateUsd.Value);
                    var rateIdr = rates.Single(r => r.Date == new DateTime(2019, 1, 28) && r.Currency == "IDR");
                    Assert.AreEqual(1000, rateIdr.Amount);
                    Assert.AreEqual(1.601m, rateIdr.Value);
                });
            }
        }

        private static StreamReader GetTestData(string name)
        {
            return new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"Ozon.Examination.Service.Tests.CzechNationalBankData.{name}"), Encoding.UTF8);
        }
    }
}