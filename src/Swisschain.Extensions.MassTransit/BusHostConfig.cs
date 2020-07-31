namespace Swisschain.Extensions.MassTransit
{
    public sealed class BusHostConfig
    {
        public BusHostConfig()
        {
            EnableMessagesAudit = true;
        }

        public bool EnableMessagesAudit { get; set; }
    }
}
