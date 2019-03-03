using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Ozon.Examination.Service.CzechNationalBank;
using Ozon.Examination.Service.Filters;
using Ozon.Examination.Service.Middleware;
using Ozon.Examination.Service.Persistence;
using Ozon.Examination.Service.Services;
using Ozon.Examination.Service.Settings;
using System;

namespace Ozon.Examination.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IRateClient, RateClient>(client =>
            {
                client.BaseAddress = new Uri(this.Configuration["CNBExchangeRateUrl"]);
            });

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<RateDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("CNBRate")))
                .BuildServiceProvider();

            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IExchangeService, ExchangeService>();
            services.AddSingleton<ISystemClock>(new SystemClock());
            services.AddHostedService<DailySynchronizationService>();

            services.Configure<DailySynchronizationOptions>(Configuration.GetSection("DailySynchronization"));
            services.Configure<ExchangeOptions>(Configuration.GetSection("ExchangeStatistics"));

            services
                .AddMvc(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.OutputFormatters.Add(new ToStringOutputFormatter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<RateDbContext>())
                context.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
