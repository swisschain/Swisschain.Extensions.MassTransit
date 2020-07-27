using Microsoft.Extensions.DependencyInjection;
using Swisschain.Extensions.MassTransit.Audit;

namespace Swisschain.Extensions.MassTransit
{
    public static class MassTransitServiceCollectionExtensions
    {
        public static IServiceCollection AddMassTransitBusHost(this IServiceCollection services)
        {
            services.AddTransient<MessageConsumptionAuditObserver>();
            services.AddTransient<MessagePublishingAuditObserver>();
            services.AddTransient<MessageSendingAuditObserver>();
            services.AddTransient<MessageRetryAuditObserver>();

            services.AddHostedService<BusHost>();

            return services;
        }
    }
}
