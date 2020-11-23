using System;
using GreenPipes.Configurators;

namespace Swisschain.Extensions.MassTransit.Retries
{
    public static class ExceptionConfiguratorExtensions
    {
        public static IExceptionConfigurator UseDefaultExceptionFilters(this IExceptionConfigurator configurator)
        {
            configurator.Ignore<ArgumentException>();
            configurator.Ignore<NullReferenceException>();
            configurator.Ignore<InvalidCastException>();
            configurator.Ignore<IndexOutOfRangeException>();
            configurator.Ignore<NotSupportedException>();
            configurator.Ignore<NotImplementedException>();

            return configurator;
        }
    }
}