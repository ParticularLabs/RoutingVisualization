using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Raven.Client;
using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json;

namespace RoutingVisualization
{
    static class Program
    {
        static void Main(string[] args)
        {
            var serviceControlDataUrl = ConfigurationManager.AppSettings["ServiceControl/RavenAddress"];
            var store = GetDocumentStore(serviceControlDataUrl);

            var modelBuilder = new ModelBuilder(GetNodeStrategy());

            var dataSource = new AllProcessedMessagesRoutedMessageSource(store);

            dataSource.RegisterListener(modelBuilder.Accept);
            var model = modelBuilder.GetModel();
            Console.WriteLine("Model generated");

            var dgml = DgmlRouteDocFactory.CreateDgml(model);
            var outputFileName = args.FirstOrDefault() ?? "route-graph";
            if (!outputFileName.EndsWith(".dgml", StringComparison.CurrentCultureIgnoreCase))
            {
                outputFileName += ".dgml";
            }

            dgml.Save(outputFileName);

            Console.WriteLine($"Created {outputFileName}");
        }

        private static IDocumentStore GetDocumentStore(string url)
        {
            var store = new DocumentStore
            {
                Url = url,
                Conventions =
                {
                    // Prevents $type from interfering with deserialization of EndpointDetails
                    CustomizeJsonSerializer = serializer => serializer.TypeNameHandling = TypeNameHandling.None
                },
            };

            store.Initialize();

            try
            {
                store.DatabaseCommands.GetBuildNumber();
            }
            catch
            {
                Console.WriteLine($"Unable to connect to the configured ServiceControl database.\nPlease check that opening {url} in the browser shows the RavenDB Management Studio.");
                Console.WriteLine($"You can change this location by adjusting the value of ServiceControl/RavenAddress in the config file:\n\t{AppDomain.CurrentDomain.SetupInformation.ConfigurationFile}");
                throw;
            }

            Console.WriteLine($"Reading messages from {store.Url}");

            return store;
        }

        private static NodeStrategy GetNodeStrategy()
        {
            // Logical Routing Strategy collapses endpoints with the same name into a single endpoint. Even if running on different hosts
            var logicalEndpoints = new LogicalRoutingNodeStrategy();
            // Physical Routing Strategy will split up endpoints that have the same name but run on different hosts
            //var physicalEndpoints = new PhysicalRoutingNodeStrategy();

            var endpoints = logicalEndpoints;
            
            // All messages of the same type that come from the same sender will be collapsed into a single message node
            var collapseFromSender = new CollaseMessagesFromSameSenderMessageNodeStrategy(endpoints);

            // All messages of the same type that go to the same receiver will be collapsed into a single message node
            //var collapseToReceiver = new CollapseMessagesToSameReceiverMessageNodeStrategy(endpoints);

            // All published events of the same type and sender will collapse into one, 
            // all non-events of the same type and sent to the same reciever collapses into one
            //var intentBasedMessageNodeStrategy = new IntentBasedMessageNodeStrategy(collapseToReceiver);
            //intentBasedMessageNodeStrategy.Add("Publish", collapseFromSender);

            var messages = collapseFromSender;

            return new NodeStrategy(endpoints, messages);
        }
    }
}
