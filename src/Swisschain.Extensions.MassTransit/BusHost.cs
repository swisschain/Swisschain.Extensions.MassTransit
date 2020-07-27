using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swisschain.Extensions.MassTransit.Audit;

namespace Swisschain.Extensions.MassTransit
{
    internal class BusHost : IHostedService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<BusHost> _logger;
        private readonly MessageConsumptionAuditObserver _consumingAuditObserver;
        private readonly MessagePublishingAuditObserver _publishingAuditObserver;
        private readonly MessageSendingAuditObserver _sendingAuditObserver;

        public BusHost(IBusControl busControl, 
            ILogger<BusHost> logger,
            MessageConsumptionAuditObserver consumingAuditObserver,
            MessagePublishingAuditObserver publishingAuditObserver,
            MessageSendingAuditObserver sendingAuditObserver)
        {
            _busControl = busControl;
            _logger = logger;
            _consumingAuditObserver = consumingAuditObserver;
            _publishingAuditObserver = publishingAuditObserver;
            _sendingAuditObserver = sendingAuditObserver;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bus host is being started...");

            await _busControl.StartAsync(cancellationToken);

            _busControl.ConnectConsumeObserver(_consumingAuditObserver);
            _busControl.ConnectPublishObserver(_publishingAuditObserver);
            _busControl.ConnectSendObserver(_sendingAuditObserver);

            _logger.LogInformation("Bus host has been started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bus host is being stopped...");

            await _busControl.StopAsync(cancellationToken);

            _logger.LogInformation("Bus host has been stopped");
        }
    }
}
