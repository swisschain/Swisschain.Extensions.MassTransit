using System;
using MassTransit.EndpointConfigurators;
using Swisschain.Extensions.MassTransit.Retries;

namespace Swisschain.Extensions.MassTransit
{
    public static class EndpointConfigurationObserverConnectorExtensions
    {
        public static IEndpointConfigurationObserverConnector UseDefaultRetries(this IEndpointConfigurationObserverConnector connector,
            IServiceProvider serviceProvider,
            Action<DefaultReceiveEndpointRetriesOptions> configOptions = null)
        {
            var options = new DefaultReceiveEndpointRetriesOptions();

            configOptions?.Invoke(options);

            connector.ConnectEndpointConfigurationObserver(new DefaultReceiveEndpointRetriesConfigurationObserver(
                serviceProvider,
                options));

            return connector;
        }
    }
}
