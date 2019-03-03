using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Ozon.Examination.Service.Services;
using Ozon.Examination.Service.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ozon.Examination.Service.Middleware
{
    public class DailySynchronizationService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ISystemClock systemClock;
        private readonly DailySynchronizationOptions options;
        private Timer timer;

        public DailySynchronizationService(IServiceScopeFactory serviceScopeFactory, ISystemClock systemClock, IOptions<DailySynchronizationOptions> options)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.systemClock = systemClock;
            this.options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var dueTime = this.options.Time.UtcDateTime.TimeOfDay - this.systemClock.UtcNow.TimeOfDay;
            if (dueTime.TotalMilliseconds < 0)
                dueTime += TimeSpan.FromDays(1);

            this.timer = new Timer(DoWork, null, dueTime, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = this.serviceScopeFactory.CreateScope())
            {
                var date = this.systemClock.UtcNow + this.options.Time.Offset;

                var exchangeService = scope.ServiceProvider.GetRequiredService<IExchangeService>();
                exchangeService.UpdateRatesAsync(date.Date).Wait();
            }
        }

        public void Dispose()
        {
            this.timer?.Dispose();
        }
    }
}
