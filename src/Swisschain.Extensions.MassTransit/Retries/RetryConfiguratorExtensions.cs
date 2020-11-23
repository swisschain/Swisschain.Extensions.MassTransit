using System;
using GreenPipes;
using GreenPipes.Configurators;

namespace Swisschain.Extensions.MassTransit.Retries
{
    public static class RetryConfiguratorExtensions
    {
        public static IRetryConfigurator UseDefaultFirstLevelRetriesIntervals(this IRetryConfigurator configurator)
        {
            configurator.Intervals(
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(3));

            return configurator;
        }

        public static IRetryConfigurator UseDefaultSecondLevelRetriesIntervals(this IRetryConfigurator configurator)
        {
            configurator.Intervals(
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(60),
                TimeSpan.FromMinutes(10),
                TimeSpan.FromMinutes(30));

            return configurator;
        }
    }
}