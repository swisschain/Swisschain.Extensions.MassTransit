using System;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.EndpointConfigurators;

namespace Swisschain.Extensions.MassTransit.Retries
{
    internal class DefaultReceiveEndpointRetriesConfigurationObserver : IEndpointConfigurationObserver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DefaultReceiveEndpointRetriesOptions _options;

        public DefaultReceiveEndpointRetriesConfigurationObserver(IServiceProvider serviceProvider, DefaultReceiveEndpointRetriesOptions options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }

        public void EndpointConfigured<T>(T configurator) where T : IReceiveEndpointConfigurator
        {
            configurator.UseScheduledRedelivery(x =>
            {
                x.Intervals(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromMinutes(30));

                ConfigureExceptions(x);
            });

            configurator.UseMessageRetry(x =>
            {
                if (_options.AuditRetries)
                {
                    x.AddRetriesAudit(_serviceProvider);
                }

                x.Intervals(
                    TimeSpan.FromMilliseconds(0),
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3));

                ConfigureExceptions(x);
            });
        }

        private static void ConfigureExceptions(IExceptionConfigurator configurator)
        {
            configurator.Ignore<ArgumentException>();
            configurator.Ignore<NullReferenceException>();
            configurator.Ignore<InvalidCastException>();
            configurator.Ignore<IndexOutOfRangeException>();
            configurator.Ignore<InvalidOperationException>();
            configurator.Ignore<NotSupportedException>();
            configurator.Ignore<NotImplementedException>();
        }
    }
}
