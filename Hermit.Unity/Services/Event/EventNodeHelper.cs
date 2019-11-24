using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Hermit
{
    public static class EventNodeHelper
    {
        private static readonly ConcurrentDictionary<string, EventNode[]> Mapping =
            new ConcurrentDictionary<string, EventNode[]>();

        public static IEnumerable<EventNode> GetEventNodes(this EventNode rootNode, string targetEndpoint)
        {
            if (string.IsNullOrEmpty(targetEndpoint)) { throw new Exception("Endpoint is not valid."); }

            if (Mapping.TryGetValue(targetEndpoint, out var nodes)) { return nodes; }

            EventNode[] ret = null;

            var cursor = rootNode;
            var endpoints = targetEndpoint.Split('.');

            for (var i = 0; i < endpoints.Length; i++)
            {
                var endpoint = endpoints[i];
                var lastElement = i == endpoints.Length - 1;
                var wildcard = endpoint.Equals("*");
                if (lastElement && wildcard) { ret = cursor.Children.Values.ToArray(); }
                else
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

            Mapping.TryAdd(targetEndpoint, ret);
            return ret;
        }
    }
}