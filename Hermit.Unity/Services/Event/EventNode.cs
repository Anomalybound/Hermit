using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Hermit
{
    public class EventNode
    {
        public string Endpoint { get; }

        public EventNode Parent { get; private set; }

        public ConcurrentDictionary<string, EventNode> Children { get; } =
            new ConcurrentDictionary<string, EventNode>();

        private ConcurrentBag<Subscription> Subscriptions { get; set; } = new ConcurrentBag<Subscription>();

        public EventNode(string endpoint)
        {
            Endpoint = endpoint;
        }

        public void SetParent(EventNode node)
        {
            Parent = node;
        }

        public ConcurrentBag<Subscription> GetSubscriptions()
        {
            return Subscriptions;
        }

        public void AddSubscription(Subscription subscription)
        {
            if (subscription == null) { throw new NullReferenceException(); }

            Subscriptions.Add(subscription);
        }

        public void RemoveSubscription(Subscription subscription)
        {
            if (subscription == null) { throw new NullReferenceException(); }

            // TODO: check performance issues
            Subscriptions = new ConcurrentBag<Subscription>(Subscriptions.Except(new[] {subscription}));
        }

        public void AddChild(EventNode node)
        {
            if (node == null) { throw new NullReferenceException(); }

            if (Children.TryAdd(node.Endpoint, node)) { node.SetParent(this); }
        }

        public static void AddSubscription(EventNode rootNode, Subscription subscription)
        {
            var targetNodes = rootNode.GetEventNodes(subscription.Endpoint);
            foreach (var targetNode in targetNodes) { targetNode.AddSubscription(subscription); }
        }

        public static void RemoveSubscription(EventNode rootNode, Subscription subscription)
        {
            var targetNodes = rootNode.GetEventNodes(subscription.Endpoint);
            foreach (var targetNode in targetNodes) { targetNode.RemoveSubscription(subscription); }
        }
    }
}