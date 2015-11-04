using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RoutingVisualization
{
    public static class DgmlRouteDocFactory
    {
        private static readonly XNamespace DgmlNamespace = XNamespace.Get("http://schemas.microsoft.com/vs/2009/dgml");

        public static XDocument CreateDgml(Model model)
        {
            return new XDocument(
                new XElement(DgmlNamespace + "DirectedGraph",
                    new XAttribute("GraphDirection", "LeftToRight"),
                    new XAttribute("Layout", "Sugiyama"),
                    new XElement(DgmlNamespace + "Nodes", Nodes(model.GetNodes())),
                    new XElement(DgmlNamespace + "Links", Links(model.GetLinks())),
                    new XElement(DgmlNamespace + "Categories", GetCategories()),
                    new XElement(DgmlNamespace + "Properties", GetProperties()),
                    new XElement(DgmlNamespace + "Styles", GetStyles())
                    )
                );
        }

        private static IEnumerable<XElement> Nodes(IEnumerable<IDictionary<string, string>> data)
        {
            return from node in data
                select new XElement(DgmlNamespace + "Node",
                    from attr in node select new XAttribute(attr.Key, attr.Value)
                    );
        }

        private static IEnumerable<XElement> Links(IEnumerable<IDictionary<string, string>> links)
        {
            return from link in links
                select new XElement(DgmlNamespace + "Link",
                    from attr in link select new XAttribute(attr.Key, attr.Value)
                    );
        }

        private static IEnumerable<XElement> GetCategories()
        {
            yield break;
        }

        private static IEnumerable<XElement> GetProperties()
        {
            yield break;
        }

        private static IEnumerable<XElement> GetStyles()
        {
            yield return MakeStyle("Node", "MessageType", "Publish",
                "Intent = 'Publish'",
                Set("Icon", @".\event32.png")
                );

            yield return MakeStyle("Node", "MessageType", "Send",
                "Intent = 'Send'",
                Set("Icon", @".\command32.png")
                );

            yield return MakeStyle("Node", "MessageType", "Reply",
                "Intent = 'Reply'",
                Set("Icon", @".\command32.png")
                );

            yield return MakeStyle("Node", "Type", "Endpoint",
                "HasCategory('Endpoint')",
                Set("Icon", @".\endpoint32.png"),
                Set("Background", "#FF8998BC")
                );

            yield return MakeStyle("Node", "Type", "Message",
                "HasCategory('Message')"
                );

            yield return MakeStyle("Link", "Intent", "Publish",
                "Intent = 'Publish'",
                Set("StrokeDashArray", "5")
                );

            yield return MakeStyle("Link", "Intent", "Reply",
                "Intent = 'Reply'",
                Set("StrokeDashArray", "1,5")
                );

            yield return MakeStyle("Link", "Intent", "Send",
                "Intent = 'Send'"
                );
        }

        private static XElement MakeStyle(string targetType, string groupLabel, string valueLabel,
            string conditionExpression, params XElement[] sets)
        {
            return new XElement(DgmlNamespace + "Style", new XAttribute("TargetType", targetType), new XAttribute("GroupLabel", groupLabel), new XAttribute("ValueLabel", valueLabel),
                new[] { new XElement(DgmlNamespace + "Condition", new XAttribute("Expression", conditionExpression)) }
                    .Concat(sets)
                );
        }

        private static XElement Set(string property, string value)
        {
            return new XElement(DgmlNamespace + "Setter",
                new XAttribute("Property", property),
                new XAttribute("Value", value)
                );
        }
    }
}