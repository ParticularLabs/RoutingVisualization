using System.Linq;

namespace RoutingVisualization
{
    public class ModelBuilder
    {
        private readonly Model _model;
        private readonly NodeStrategy _nodeStrategy;

        public ModelBuilder(NodeStrategy nodeStrategy)
        {
            _model = new Model();
            _nodeStrategy = nodeStrategy;
        }

        public void Accept(ProcessedMessage message)
        {

            var intent = message.Headers["NServiceBus.MessageIntent"];
            if (intent == "Subscribe" || intent == "Unsubscribe")
                return;

            var senderId = _nodeStrategy.GetNodeId(message.MessageMetadata.SendingEndpoint);
            var senderNode = _model.GetEndpoint(senderId);
            senderNode["Label"] = message.MessageMetadata.SendingEndpoint.Name;

            var receiverId = _nodeStrategy.GetNodeId(message.MessageMetadata.ReceivingEndpoint);
            var receiverNode = _model.GetEndpoint(receiverId);
            receiverNode["Label"] = message.MessageMetadata.ReceivingEndpoint.Name;

            var messageId = _nodeStrategy.GetNodeId(message);
            var messageNode = _model.GetMessage(messageId);
            messageNode["Intent"] = intent;
            messageNode["Label"] = message.MessageMetadata.MessageType.Split('.').Last();

            var senderLink = _model.GetLink(senderId, messageId);
            senderLink["Intent"] = intent;

            var receiverLink = _model.GetLink(messageId, receiverId);
            receiverLink["Intent"] = intent;
        }

        public Model GetModel()
        {
            return _model;
        }
    }
}