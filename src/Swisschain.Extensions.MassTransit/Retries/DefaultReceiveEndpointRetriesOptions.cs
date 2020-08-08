namespace Swisschain.Extensions.MassTransit.Retries
{
    public class DefaultReceiveEndpointRetriesOptions
    {
        public DefaultReceiveEndpointRetriesOptions()
        {
            AuditRetries = true;
        }

        public bool AuditRetries { get; set; }
    }
}
