using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Swisschain.Extensions.MassTransit.Audit
{
    internal sealed class MessagePublishingAuditObserver : IPublishObserver
    {
        private readonly ILogger<MessagePublishingAuditObserver> _logger;

        public MessagePublishingAuditObserver(ILogger<MessagePublishingAuditObserver> logger)
        {
            _logger = logger;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            _logger.LogInformation(
                "Message is being published {@context}",
                new
                {
                    MessageId = context.MessageId,
                    ConversationId = context.ConversationId,
                    CorrelationId = context.CorrelationId,
                    SentTime = context.SentTime,
                    Message = context.Message
                });

            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception,
                "Message publishing has been failed {@context}",
                new
                {
                    MessageId = context.MessageId
                });

            return Task.CompletedTask;
        }
    }
}
