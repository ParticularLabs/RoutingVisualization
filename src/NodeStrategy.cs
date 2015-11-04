using System.Text.RegularExpressions;

namespace RoutingVisualization
{
    public abstract class NodeStrategy
    {
        public abstract string GetNodeId(EndpointDetails details);

        public string GetNodeId(ProcessedMessage message)
        {
            return ToNodeName(GetNodeId(message.MessageMetadata.SendingEndpoint), message.Headers["NServiceBus.MessageIntent"], message.MessageMetadata.MessageType);
        }

        protected static string ToNodeName(params string[] parts)
        {
            return Regex.Replace(string.Join("_", parts), "[-.]", "_").ToLower();
        }
    }

    class PhysicalRoutingNodeStrategy : NodeStrategy
    {
        public override string GetNodeId(EndpointDetails details)
        {
            return ToNodeName(details.Host, details.Name);
        }
    }

    class LogicalRoutingNodeStrategy : NodeStrategy
    {
        public override string GetNodeId(EndpointDetails details)
        {
            return ToNodeName(details.Name);
        }
    }

}