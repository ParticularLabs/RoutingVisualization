using System.Collections.Generic;
using NUnit.Framework;

namespace RoutingVisualization.UnitTests
{
    public class ModelBuilderTests
    {
        [Test]
        public void CanAcceptMessageWithoutHeaders()
        {
            var endpointNodeStrategy = new LogicalRoutingNodeStrategy();
            var messageNodeStrategy = new CollapseMessagesToSameReceiverMessageNodeStrategy(endpointNodeStrategy);
            var nodeStrategy = new NodeStrategy(endpointNodeStrategy, messageNodeStrategy);
            var modelBuilder = new ModelBuilder(nodeStrategy);

            var message = new ProcessedMessage
            {
                Headers = new Dictionary<string, string>()
            };

            Assert.DoesNotThrow(
                () => modelBuilder.Accept(message)
            );
        }

        [Test]
        public void CanAcceptMessageWithoutMessageType()
        {
            var endpointNodeStrategy = new LogicalRoutingNodeStrategy();
            var messageNodeStrategy = new CollapseMessagesToSameReceiverMessageNodeStrategy(endpointNodeStrategy);
            var nodeStrategy = new NodeStrategy(endpointNodeStrategy, messageNodeStrategy);
            var modelBuilder = new ModelBuilder(nodeStrategy);

            var message = new ProcessedMessage
            {
                Headers = new Dictionary<string, string>
                {
                    ["NServiceBus.Intent"] = "Send"
                }
            };

            Assert.DoesNotThrow(
                () => modelBuilder.Accept(message)
            );
        }
    }
}