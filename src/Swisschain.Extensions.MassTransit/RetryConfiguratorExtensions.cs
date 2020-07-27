using System;
using GreenPipes.Configurators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swisschain.Extensions.MassTransit.Audit;

namespace Swisschain.Extensions.MassTransit
{
    public static class RetryConfiguratorExtensions
    {
        public static IRetryConfigurator AddRetriesAudit(this IRetryConfigurator configurator, IServiceProvider provider)
        {
            configurator.ConnectRetryObserver(provider.GetRequiredService<MessageRetryAuditObserver>());

            return configurator;
        }
    }
}
