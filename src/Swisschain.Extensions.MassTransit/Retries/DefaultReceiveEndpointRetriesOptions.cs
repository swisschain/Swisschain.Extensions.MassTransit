using System;
using GreenPipes.Configurators;

namespace Swisschain.Extensions.MassTransit.Retries
{
    public class DefaultReceiveEndpointRetriesOptions
    {
        public DefaultReceiveEndpointRetriesOptions()
        {
            AuditRetries = true;
            
            FirstLevelRetriesConfigurator = x =>
            {
                 x.UseDefaultFirstLevelRetriesIntervals();
                 x.UseDefaultExceptionFilters();
            };

            SecondLevelRetriesConfigurator = x =>
            {
                x.UseDefaultSecondLevelRetriesIntervals();
                x.UseDefaultExceptionFilters();
            };
        }

        public bool AuditRetries { get; set; }
        public Action<IRetryConfigurator> FirstLevelRetriesConfigurator { get; private set; }
        public Action<IRetryConfigurator> SecondLevelRetriesConfigurator { get; private set; }

        public DefaultReceiveEndpointRetriesOptions OverrideFirstLevelRetries(Action<IRetryConfigurator> configurator)
        {
            FirstLevelRetriesConfigurator = configurator ?? throw new ArgumentNullException(nameof(configurator));

            return this;
        }

        public DefaultReceiveEndpointRetriesOptions OverrideSecondLevelRetries(Action<IRetryConfigurator> configurator)
        {
            SecondLevelRetriesConfigurator = configurator ?? throw new ArgumentNullException(nameof(configurator));

            return this;
        }
    }
}
