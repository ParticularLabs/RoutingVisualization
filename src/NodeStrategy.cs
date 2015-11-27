using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RoutingVisualization
{
    public class NodeStrategy : INodeStrategy<EndpointDetails>, INodeStrategy<ProcessedMessage>
    {
        private INodeStrategy<EndpointDetails> _endpointNodeStrategy;
        private INodeStrategy<ProcessedMessage> _messageNodeStrategy;

        public NodeStrategy(INodeStrategy<EndpointDetails> endpointNodeStrategy, INodeStrategy<ProcessedMessage> messageNodeStrategy)
        {
            _endpointNodeStrategy = endpointNodeStrategy;
            _messageNodeStrategy = messageNodeStrategy;
        }

        public string GetNodeId(EndpointDetails details)
        {
            return _endpointNodeStrategy.GetNodeId(details);
        }

        public string GetNodeId(ProcessedMessage details)
        {
            return _messageNodeStrategy.GetNodeId(details);
        }
    }

    public interface INodeStrategy<in T> 
    {
        string GetNodeId(T details);
    }

    public abstract class NodeStrategy<T> : INodeStrategy<T>
    {
        public abstract string GetNodeId(T details);

        protected static string ToNodeName(params string[] parts)
        {
            return Regex.Replace(string.Join("_", parts), "[-.]", "_").ToLower();
        }
    }
    

    class PhysicalRoutingNodeStrategy : NodeStrategy<EndpointDetails>
    {
        public override string GetNodeId(EndpointDetails details)
        {
            return ToNodeName(details.Host, details.Name);
        }
    }

    class LogicalRoutingNodeStrategy : NodeStrategy<EndpointDetails>
    {
        public override string GetNodeId(EndpointDetails details)
        {
            return ToNodeName(details.Name);
        }
    }

    class CollaseMessagesFromSameSenderMessageNodeStrategy : NodeStrategy<ProcessedMessage>
    {
        private NodeStrategy<EndpointDetails> _endpointNodeStrategy;

        public CollaseMessagesFromSameSenderMessageNodeStrategy(NodeStrategy<EndpointDetails> endpointNodeStrategy)
        {
            _endpointNodeStrategy = endpointNodeStrategy;
        }

        public override string GetNodeId(ProcessedMessage details)
        {
            var intent = details.Headers["NServiceBus.MessageIntent"];
            var sendingEndpointNodeId = _endpointNodeStrategy.GetNodeId(details.MessageMetadata.SendingEndpoint);
            return ToNodeName(sendingEndpointNodeId, intent, details.MessageMetadata.MessageType);
        }
    }

    class CollapseMessagesToSameReceiverMessageNodeStrategy : NodeStrategy<ProcessedMessage>
    {
        private NodeStrategy<EndpointDetails> _endpointNodeStrategy;

        public CollapseMessagesToSameReceiverMessageNodeStrategy(NodeStrategy<EndpointDetails> endpointNodeStrategy)
        {
            _endpointNodeStrategy = endpointNodeStrategy;
        }

        public override string GetNodeId(ProcessedMessage details)
        {
            var intent = details.Headers["NServiceBus.MessageIntent"];
            var receivingEndpointNodeId = _endpointNodeStrategy.GetNodeId(details.MessageMetadata.ReceivingEndpoint);
            return ToNodeName(receivingEndpointNodeId, intent, details.MessageMetadata.MessageType);
        }
    }

    class IntentBasedMessageNodeStrategy : INodeStrategy<ProcessedMessage>
    {
        private IDictionary<string, INodeStrategy<ProcessedMessage>> _strategyMap = new Dictionary<string, INodeStrategy<ProcessedMessage>>(StringComparer.InvariantCultureIgnoreCase);
        private INodeStrategy<ProcessedMessage> _defaultStrategy;

        public IntentBasedMessageNodeStrategy(INodeStrategy<ProcessedMessage> defaultStrategy)
        {
            _defaultStrategy = defaultStrategy;
        }

        public string GetNodeId(ProcessedMessage details)
        {
            var intent = details.Headers["NServiceBus.MessageIntent"];

            INodeStrategy<ProcessedMessage> strategy;
            if (!_strategyMap.TryGetValue(intent, out strategy))
                strategy = _defaultStrategy;

            return strategy.GetNodeId(details);
        }

        public void Add(string intent, INodeStrategy<ProcessedMessage> strategy)
        {
            _strategyMap.Add(intent, strategy);
        }
    }
}