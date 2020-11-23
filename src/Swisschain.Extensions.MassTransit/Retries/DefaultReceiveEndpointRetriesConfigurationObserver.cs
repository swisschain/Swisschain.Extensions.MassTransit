using System;
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
                _options.SecondLevelRetriesConfigurator.Invoke(x);
            });

            configurator.UseMessageRetry(x =>
            {
                if (_options.AuditRetries)
                {
                    x.AddRetriesAudit(_serviceProvider);
                }

                _options.FirstLevelRetriesConfigurator.Invoke(x);
            });
        }
    }
}
