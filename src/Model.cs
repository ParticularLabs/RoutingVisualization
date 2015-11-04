using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RoutingVisualization
{
    public class Model
    {
        private readonly ConcurrentDictionary<string, IDictionary<string, string>> _endpoints = new ConcurrentDictionary<string, IDictionary<string, string>>();
        private readonly ConcurrentDictionary<string, IDictionary<string, string>> _messages = new ConcurrentDictionary<string, IDictionary<string, string>>();
        private readonly ConcurrentDictionary<Tuple<string, string>, IDictionary<string, string>> _links = new ConcurrentDictionary<Tuple<string, string>, IDictionary<string, string>>();

        public IDictionary<string, string> GetEndpoint(string endpointId)
        {
            return _endpoints.GetOrAdd(endpointId, id => new Dictionary<string, string>
            {
                ["Id"] = id,
                ["Category"] = "Endpoint"
            });
        }

        public IDictionary<string, string> GetMessage(string messageId)
        {
            return _messages.GetOrAdd(messageId, id => new Dictionary<string, string>
            {
                ["Id"] = id,
                ["Category"] = "Message"
            });
        }

        public IDictionary<string, string> GetLink(string source, string target)
        {
            return _links.GetOrAdd(Tuple.Create(source, target), id => new Dictionary<string, string>
            {
                ["Source"] = source,
                ["Target"] = target
            });
        }

        public IEnumerable<IDictionary<string, string>> GetNodes()
        {
            return _endpoints.Values.Concat(_messages.Values);
        }

        public IEnumerable<IDictionary<string, string>> GetLinks()
        {
            return _links.Values;
        }
    }
}