using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Metadata;
using MassTransit.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Swisschain.Extensions.MassTransit;

namespace TestApp
{
    public class MyEvent
    {
        public string Value { get; set; }
        public int X { get; set; }
    }

    class MyEventConsumer : IConsumer<MyEvent>
    {
        private readonly ILogger<MyEventConsumer> _logger;

        public MyEventConsumer(ILogger<MyEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MyEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("Message is being processed {@Message}", message);

            throw new InvalidOperationException("aaa");
        }
    }

    public abstract class UtcRecurringSchedule : RecurringSchedule
    {
        protected UtcRecurringSchedule()
        {
            this.ScheduleId = TypeMetadataCache.GetShortName(this.GetType());
            this.ScheduleGroup = this.GetType().GetTypeInfo().Assembly.FullName.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            this.TimeZoneId = TimeZoneInfo.Utc.Id;
            this.StartTime = DateTimeOffset.UtcNow;
        }

        public MissedEventPolicy MisfirePolicy { get; protected set; }
        public string TimeZoneId { get; protected set; }
        public DateTimeOffset StartTime { get; protected set; }
        public DateTimeOffset? EndTime { get; protected set; }
        public string ScheduleId { get; private set; }
        public string ScheduleGroup { get; private set; }
        public string CronExpression { get; protected set; }
        public string Description { get; protected set; }
    }

    public class MyRecurringSchedule : UtcRecurringSchedule
    {
        public MyRecurringSchedule()
        {
            MisfirePolicy = MissedEventPolicy.Send;
            CronExpression = "0/10 0 0 ? * * *";
        }
    }

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            var schedulerEndpoint = new Uri("queue:scheduler");

            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Console();

            Log.Logger = loggerConfig.CreateLogger();

            var loggerFactory = new LoggerFactory().AddSerilog();

            services.AddSingleton<ILoggerFactory>(loggerFactory);
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddTransient<MyEventConsumer>();

            services.AddMassTransit(x =>
            {
                x.AddMessageScheduler(schedulerEndpoint);
                
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://localhost:5673", h =>
                    {
                        h.Username("examples");
                        h.Password("examples");
                    });

                    cfg.UseMessageScheduler(schedulerEndpoint);

                    cfg.UseDefaultRetries(context);
                        
                    cfg.ReceiveEndpoint("submit-order", e =>
                    {
                        e.Consumer<MyEventConsumer>(context);
                    });
                });
            });

            services.AddMassTransitBusHost();

            //services.AddMassTransit(x =>
            //{
            //    x.AddMessageScheduler(schedulerEndpoint);

            //    x.UsingRabbitMq((context, cfg) => 
            //    {
            //        cfg.Host("localhost", h =>
            //        {
            //            h.Username("examples");
            //            h.Password("examples");
            //        });

            //        cfg.UseMessageScheduler(schedulerEndpoint);

            //        cfg.ConnectEndpointConfigurationObserver(new ReceiveEndpointConfigurationObserver());

            //        cfg.ReceiveEndpoint("submit-order", e =>
            //        {
            //            e.Consumer<MyEventConsumer>();
            //        });
            //    });
            //});

            await using var serviceProvider = services.BuildServiceProvider();

            var hosts = serviceProvider.GetServices<IHostedService>();
            var bus = serviceProvider.GetRequiredService<IBusControl>();
            
            foreach (var host in hosts)
            {
                await host.StartAsync(CancellationToken.None);
            }
            //var result = bus.GetProbeResult();

            //Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            //await bus.StartAsync();
            
            await bus.Publish(
                new MyEvent
                {
                    Value = "Hello, World.",
                    X = 10
                });

            Console.ReadLine();

            await bus.StopAsync();
        }
    }

}
