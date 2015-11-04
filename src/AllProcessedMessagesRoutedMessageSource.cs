using System;
using Raven.Client;

namespace RoutingVisualization
{
    class AllProcessedMessagesRoutedMessageSource
    {
        private readonly IDocumentStore _store;

        public AllProcessedMessagesRoutedMessageSource(IDocumentStore store)
        {
            _store = store;
        }

        public void RegisterListener(Action<ProcessedMessage> onNext)
        {
            using (var session = _store.OpenSession())
            using (var stream = session.Advanced.Stream<ProcessedMessage>("ProcessedMessage"))
            {
                var count = 0;
                while (stream.MoveNext())
                {
                    Console.Write($"\r{count++}");
                    onNext(stream.Current.Document);
                }
                Console.WriteLine();
            }
        }
    }
}