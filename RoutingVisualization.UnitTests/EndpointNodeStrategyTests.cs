using NUnit.Framework;

namespace RoutingVisualization.UnitTests
{
    public class LogicalRoutingNodeStrategyTests
    {
        [Test]
        public void NodeIdForANullEndpointShouldBeNull()
        {
            var logicalRoutingNodeStrategy = new LogicalRoutingNodeStrategy();

            var nodeId = logicalRoutingNodeStrategy.GetNodeId(null);

            Assert.IsNull(nodeId);
        }
    }
}