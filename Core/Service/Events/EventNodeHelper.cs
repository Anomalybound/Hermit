using System;
using System.Collections.Generic;

namespace Hermit.Service.Events
{
    public static class EventNodeHelper
    {
        public static IEnumerable<EventNode> GetEventNodes(this EventNode rootNode, string targetEndpoint)
        {
            if (string.IsNullOrEmpty(targetEndpoint)) { throw new Exception("Endpoint is not valid."); }

            EventNode[] ret = null;

            var cursor = rootNode;
            var endpoints = targetEndpoint.Split('.');

            for (var i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                var lastElement = i == endpoints.Length - 1;
                var wildcard = endpoint.Equals("*");

                if (wildcard && !lastElement) { throw new Exception($"Endpoint is not valid: {targetEndpoint}"); }

                if (lastElement && wildcard)
                {
                    var list = new List<EventNode>();
                    AddChildren(cursor, list);
                    ret = list.ToArray();
                } else
                {
                    if (!cursor.Children.TryGetValue(endpoint, out var child))
                    {
                        child = new EventNode(endpoint);
                        cursor.AddChild(child);
                    }

                    cursor = child;

                    if (!lastElement) { continue; }

                    ret = new[] {cursor};
                }
            }

            return ret;
        }

        private static void AddChildren(EventNode node, ICollection<EventNode> list)
        {
            foreach (var nodeChild in node.Children.Values)
            {
                list.Add(nodeChild);
                AddChildren(nodeChild, list);
            }
        }
    }
}
