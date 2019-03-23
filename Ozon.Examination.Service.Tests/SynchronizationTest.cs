using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Moq;
using NUnit.Framework;
using Ozon.Examination.Service.Middleware;
using Ozon.Examination.Service.Services;
using Ozon.Examination.Service.Settings;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Tests
{
    public class SynchronizationTest
    {
        [Test]
        public async Task DailyUpdateRates_StartNow_GetUpdateRates()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddHostedService<DailySynchronizationService>();

            var systemClock = new Mock<ISystemClock>(MockBehavior.Loose);
            
            systemClock
                .SetupSequence(m => m.UtcNow)
                // current time on start background service
                .Returns(new DateTimeOffset(2019, 3, 23, 23, 29, 59, 900, TimeSpan.Zero))
                // curent time on synchronization process
                .Returns(new DateTimeOffset(2019, 3, 23, 23, 30, 00, 000, TimeSpan.Zero));
            services.AddSingleton(systemClock.Object);

            // time on start synchronization process with time zone
            services.Configure<DailySynchronizationOptions>(o => o.Time = DateTimeOffset.Parse("00:30:00 +1", CultureInfo.InvariantCulture));

            var exchangeService = new Mock<IExchangeService>(MockBehavior.Loose);
            services.AddSingleton(exchangeService.Object);

            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetService<IHostedService>();

            await service.StartAsync(CancellationToken.None);
            await Task.Delay(200);
            await service.StopAsync(CancellationToken.None);

            exchangeService.Verify(m => m.UpdateRatesAsync(
                It.IsIn(new DateTime(2019, 3, 24))),
                Times.Once());
        }
    }
}
