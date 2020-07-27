using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Swisschain.Extensions.MassTransit.Audit
{
    sealed class MessageRetryAuditObserver : IRetryObserver
    {
        private readonly ILogger<MessageRetryAuditObserver> _logger;

        public MessageRetryAuditObserver(ILogger<MessageRetryAuditObserver> logger)
        {
            _logger = logger;
        }

        public Task PostCreate<T>(RetryPolicyContext<T> context) where T : class, PipeContext
        {
            return Task.CompletedTask;
        }

        public Task PostFault<T>(RetryContext<T> context) where T : class, PipeContext
        {
            return Task.CompletedTask;
        }

        public Task PreRetry<T>(RetryContext<T> context) where T : class, PipeContext
        {
            var c = (ConsumeContext)context.Context;

            _logger.LogInformation("Message is being retried {@context}", new
            {
                MessageId = c.MessageId,
                RetryAttempt = context.RetryAttempt
            });

            return Task.CompletedTask;
        }

        public Task RetryFault<T>(RetryContext<T> context) where T : class, PipeContext
        {
            return Task.CompletedTask;
        }

        public Task RetryComplete<T>(RetryContext<T> context) where T : class, PipeContext
        {
            return Task.CompletedTask;
        }
    }
}
