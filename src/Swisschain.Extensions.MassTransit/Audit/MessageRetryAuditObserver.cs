using System.Reflection;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Swisschain.Extensions.MassTransit.Audit
{
    sealed class MessageRetryAuditObserver : IRetryObserver
    {
        private static readonly PropertyInfo ConsumeContextMessageProperty;

        private readonly ILogger<MessageRetryAuditObserver> _logger;

        static MessageRetryAuditObserver()
        {
            ConsumeContextMessageProperty = typeof(ConsumeContext<>).GetProperty("Message", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        }

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
            var consumeContext = (ConsumeContext)context.Context;
            var message = ConsumeContextMessageProperty?.GetValue(context);

            _logger.LogWarning("Message is being retried {@context}", new
            {
                MessageId = consumeContext.MessageId,
                ConversationId = consumeContext.ConversationId,
                CorrelationId = consumeContext.CorrelationId,
                RedeliveryCount = consumeContext.GetRedeliveryCount(),
                SentTime = consumeContext.SentTime,
                Message = message,
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
