using System;
using GreenPipes;
using MassTransit;
using MassTransit.EndpointConfigurators;

namespace Swisschain.Extensions.MassTransit.Retries
{
    internal class DefaultReceiveEndpointRetriesConfigurationObserver : IEndpointConfigurationObserver
    {
        public void EndpointConfigured<T>(T configurator) where T : IReceiveEndpointConfigurator
        {
            configurator.UseScheduledRedelivery(r =>
                r.Intervals(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromMinutes(30)));

            configurator.UseMessageRetry(y =>
            {
                y.Intervals(
                    TimeSpan.FromMilliseconds(0),
                    TimeSpan.FromMilliseconds(100),
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3));
            });
        }
    }
}
