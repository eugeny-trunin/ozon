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
            // calculate through how much time you need to start synchronization process
            var dueTime = this.options.Time.UtcDateTime.TimeOfDay - this.systemClock.UtcNow.TimeOfDay;
            // if negative value, synchronization process will be started next day
            if (dueTime.TotalMilliseconds < 0)
                dueTime += TimeSpan.FromDays(1);

            // start timer through the calculated period with repetition every day
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
                // calculating date of start in the context of an time zone from start time parameter
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
