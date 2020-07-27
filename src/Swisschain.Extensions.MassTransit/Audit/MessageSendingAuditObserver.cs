using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Swisschain.Extensions.MassTransit.Audit
{
    internal sealed class MessageSendingAuditObserver : ISendObserver
    {
        private readonly ILogger<MessageSendingAuditObserver> _logger;

        public MessageSendingAuditObserver(ILogger<MessageSendingAuditObserver> logger)
        {
            _logger = logger;
        }

        public Task PreSend<T>(SendContext<T> context) where T : class
        {
            _logger.LogInformation(
                "Message is being sent {@context}",
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

        public Task PostSend<T>(SendContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception,
                "Message sending has been failed {@context}",
                new
                {
                    MessageId = context.MessageId
                });

            return Task.CompletedTask;
        }
    }
}
