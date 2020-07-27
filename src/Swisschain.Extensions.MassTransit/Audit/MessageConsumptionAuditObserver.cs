using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Swisschain.Extensions.MassTransit.Audit
{
    internal sealed class MessageConsumptionAuditObserver : IConsumeObserver
    {
        private readonly ILogger<MessageConsumptionAuditObserver> _logger;

        public MessageConsumptionAuditObserver(ILogger<MessageConsumptionAuditObserver> logger)
        {
            _logger = logger;
        }

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            _logger.LogInformation(
                "Message is being consumed {@context}",
                new
                {
                    MessageId = context.MessageId,
                    ConversationId = context.ConversationId,
                    CorrelationId = context.CorrelationId,
                    RedeliveryCount = context.GetRedeliveryCount(),
                    SentTime = context.SentTime,
                    Message = context.Message
                });

            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            _logger.LogInformation(
                "Message has been consumed {@context}",
                new
                {
                    MessageId = context.MessageId,
                    ElapsedTime = context.ReceiveContext.ElapsedTime
                });

            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception,
                "Message consumption has been failed {@context}",
                new
                {
                    MessageId = context.MessageId,
                    ElapsedTime = context.ReceiveContext.ElapsedTime
                });

            return Task.CompletedTask;
        }
    }
}
