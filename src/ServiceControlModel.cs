using System.Collections.Generic;

namespace RoutingVisualization
{
    public class ProcessedMessage
    {
        public MessageMetadata MessageMetadata { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }

    public class EndpointDetails
    {
        public string Name { get; set; }
        public string Host { get; set; }
    }

    public class MessageMetadata
    {
        public string MessageType { get; set; }
        public EndpointDetails SendingEndpoint { get; set; }
        public EndpointDetails ReceivingEndpoint { get; set; }
    }
}