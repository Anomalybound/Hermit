using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Hermit.Service.Events
{
    public static class SubscriptionHelper
    {
        private static readonly ConcurrentDictionary<Type, Subscription[]> MethodCaches =
            new ConcurrentDictionary<Type, Subscription[]>();

        public static IEnumerable<Subscription> FindSubscriber(object obj)
        {
            if (obj == null) { throw new ArgumentNullException(); }

            var type = obj.GetType();
            if (MethodCaches.TryGetValue(type, out var subscriptions)) { return subscriptions; }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            var methodInfos = type.GetMethods(flags);
            var list = new List<Subscription>();
            for (var i = 0; i < methodInfos.Length; i++)
            {
                var methodInfo = methodInfos[i];
                var subscribeAttribute = methodInfo.GetCustomAttribute<SubscribeAttribute>();
                if (subscribeAttribute == null) { continue; }

                var endpoint = subscribeAttribute.Endpoint;
                var sticky = subscribeAttribute.Sticky;
                var mode = subscribeAttribute.Mode;
                var priority = subscribeAttribute.Priority;

                var sub = new Subscription(endpoint, sticky, priority, mode, methodInfo, obj);
                list.Add(sub);
            }

            var ret = list.ToArray();
            return MethodCaches.TryAdd(type, ret) ? ret : null;
        }
    }
}
