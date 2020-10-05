using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Swisschain.Extensions.MassTransit.Audit
{
    sealed class MessageRetryAuditObserver : IRetryObserver
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo> ConsumeContextMessagePropertiesOfT;

        private readonly ILogger<MessageRetryAuditObserver> _logger;

        static MessageRetryAuditObserver()
        {
            ConsumeContextMessagePropertiesOfT = new ConcurrentDictionary<Type, PropertyInfo>();
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
            var consumeContext = (ConsumeContext)context.Context;
            var message = GetMessage(consumeContext);

            _logger.LogWarning("Message processing has been failed {@context}", new
            {
                MessageId = consumeContext.MessageId,
                ConversationId = consumeContext.ConversationId,
                CorrelationId = consumeContext.CorrelationId,
                RedeliveryCount = consumeContext.GetRedeliveryCount(),
                SentTime = consumeContext.SentTime,
                Message = message,
                MessageType = message?.GetType(),
                RetryAttempt = context.RetryAttempt,
                Exception = context.Exception
            });

            return Task.CompletedTask;
        }

        public Task PreRetry<T>(RetryContext<T> context) where T : class, PipeContext
        {
            var consumeContext = (ConsumeContext)context.Context;
            var message = GetMessage(consumeContext);

            _logger.LogWarning("Message is being retried {@context}", new
            {
                MessageId = consumeContext.MessageId,
                ConversationId = consumeContext.ConversationId,
                CorrelationId = consumeContext.CorrelationId,
                RedeliveryCount = consumeContext.GetRedeliveryCount(),
                SentTime = consumeContext.SentTime,
                Message = message,
                MessageType = message?.GetType(),
                RetryAttempt = context.RetryAttempt
            });

            return Task.CompletedTask;
        }

        public Task RetryFault<T>(RetryContext<T> context) where T : class, PipeContext
        {
            var consumeContext = (ConsumeContext)context.Context;
            var message = GetMessage(consumeContext);

            _logger.LogWarning("Message retries has been exhausted {@context}", new
            {
                MessageId = consumeContext.MessageId,
                ConversationId = consumeContext.ConversationId,
                CorrelationId = consumeContext.CorrelationId,
                RedeliveryCount = consumeContext.GetRedeliveryCount(),
                SentTime = consumeContext.SentTime,
                Message = message,
                MessageType = message?.GetType(),
                RetryAttempt = context.RetryAttempt,
                Exception = context.Exception
            });

            return Task.CompletedTask;
        }

        public Task RetryComplete<T>(RetryContext<T> context) where T : class, PipeContext
        {
            return Task.CompletedTask;
        }

        private static object GetMessage(ConsumeContext consumeContext)
        {
            var consumeContextMessagePropertyOfT = ConsumeContextMessagePropertiesOfT.GetOrAdd(
                consumeContext.GetType(),
                x =>
                {
                    var messageType = consumeContext
                        .GetType()
                        .GetGenericArguments()
                        .Single();
                    var genericConsumeContextType = typeof(ConsumeContext<>).MakeGenericType(messageType);
                    var messageProperty = genericConsumeContextType.GetProperty("Message", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

                    return messageProperty;
                });

            return consumeContextMessagePropertyOfT.GetValue(consumeContext);
        }
    }
}
