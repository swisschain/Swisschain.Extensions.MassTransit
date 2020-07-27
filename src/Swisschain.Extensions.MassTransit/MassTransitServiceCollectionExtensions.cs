using Microsoft.Extensions.DependencyInjection;

namespace Swisschain.Extensions.MassTransit
{
    public static class MassTransitServiceCollectionExtensions
    {
        public static IServiceCollection AddMassTransitBusHost(this IServiceCollection services)
        {
            services.AddHostedService<BusHost>();

            return services;
        }
    }
}
