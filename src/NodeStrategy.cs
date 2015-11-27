using System.Text.RegularExpressions;

namespace RoutingVisualization
{
    public abstract class NodeStrategy
    {
        public abstract string GetNodeId(EndpointDetails details);

        public string GetNodeId(ProcessedMessage message)
        {
            var intent = message.Headers["NServiceBus.MessageIntent"];

            if (intent == "Publish")
            {
                return ToNodeName(GetNodeId(message.MessageMetadata.SendingEndpoint), intent,
                    message.MessageMetadata.MessageType);
            }
            else
            {
                return ToNodeName(GetNodeId(message.MessageMetadata.ReceivingEndpoint), intent,
                    message.MessageMetadata.MessageType);
            }

            //return ToNodeName(GetNodeId(message.MessageMetadata.SendingEndpoint), intent, message.MessageMetadata.MessageType);
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