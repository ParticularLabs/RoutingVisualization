using System;
using System.Configuration;
using System.Linq;
using Raven.Client.Document;
using Raven.Imports.Newtonsoft.Json;

namespace RoutingVisualization
{
    static class Program
    {
        static void Main(string[] args)
        {
            var store = new DocumentStore
            {
                Url = ConfigurationManager.AppSettings["ServiceControl/RavenAddress"],
                Conventions =
                {
                    // Prevents $type from interfering with deserialization of EndpointDetails
                    CustomizeJsonSerializer = serializer => serializer.TypeNameHandling = TypeNameHandling.None
                },
            };

            store.Initialize();
            Console.WriteLine($"Reading messages from {store.Url}");

            var modelBuilder = new ModelBuilder(
                // Physical Routing will split up endpoints that have the same name but are on different hosts
                // new PhysicalRoutingNodeStrategy()
                new LogicalRoutingNodeStrategy()
            );

            var dataSource = new AllProcessedMessagesRoutedMessageSource(store);

            dataSource.RegisterListener(modelBuilder.Accept);
            var model = modelBuilder.GetModel();
            Console.WriteLine("Model generated");

            var outputFileName = args.FirstOrDefault() ?? "route-graph.dgml";
            var dgml = DgmlRouteDocFactory.CreateDgml(model);
            dgml.Save(outputFileName);

            Console.WriteLine($"Created {outputFileName}");
        }
    }
}
