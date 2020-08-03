using MassTransit.EndpointConfigurators;
using Swisschain.Extensions.MassTransit.Retries;

namespace Swisschain.Extensions.MassTransit
{
    public static class EndpointConfigurationObserverConnectorExtensions
    {
        public static IEndpointConfigurationObserverConnector UseDefaultRetries(this IEndpointConfigurationObserverConnector connector)
        {
            connector.ConnectEndpointConfigurationObserver(new DefaultReceiveEndpointRetriesConfigurationObserver());

            return connector;
        }
    }
}
