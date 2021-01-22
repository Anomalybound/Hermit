using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hermit.Service.Messages
{
    internal class Subscriptions
    {
        private readonly List<Subscription> _allSubscriptions = new List<Subscription>();
        private int _subscriptionsChangeCounter;

        private readonly ThreadLocal<int> _localSubscriptionRevision =
            new ThreadLocal<int>(() => 0, true);

        private readonly ThreadLocal<List<Subscription>> _localSubscriptions =
            new ThreadLocal<List<Subscription>>(() => new List<Subscription>(), true);

        private bool _disposed;

        internal Guid Register<T>(TimeSpan throttleBy, Action<T> action)
        {
            var type = typeof(T);
            var key = Guid.NewGuid();
            var subscription = new Subscription(type, key, throttleBy, action);

            lock (_allSubscriptions)
            {
                _allSubscriptions.Add(subscription);
                _subscriptionsChangeCounter++;
            }

            return key;
        }

        internal void UnRegister(Guid token)
        {
            lock (_allSubscriptions)
            {
                var idx = _allSubscriptions.FindIndex(s => s.Token == token);

                if (idx < 0) { return; }

                var subscription = _allSubscriptions[idx];

                _allSubscriptions.RemoveAt(idx);

                for (var i = 0; i < _localSubscriptions.Values.Count; i++)
                {
                    var threadLocal = _localSubscriptions.Values[i];
                    var localIdx = threadLocal.IndexOf(subscription);
                    if (localIdx < 0) { continue; }

                    threadLocal.RemoveAt(localIdx);
                }

                _subscriptionsChangeCounter++;
            }
        }

        internal void Clear(bool dispose)
        {
            lock (_allSubscriptions)
            {
                if (_disposed) { return; }

                _allSubscriptions.Clear();

                for (var i = 0; i < _localSubscriptions.Values.Count; i++) { _localSubscriptions.Values[i].Clear(); }

                if (dispose)
                {
                    _localSubscriptionRevision.Dispose();
                    _localSubscriptions.Dispose();
                    _disposed = true;
                }
                else { _subscriptionsChangeCounter++; }
            }
        }

        internal bool IsRegistered(Guid token)
        {
            lock (_allSubscriptions) { return _allSubscriptions.Any(s => s.Token == token); }
        }

        internal List<Subscription> GetTheLatestSubscriptions()
        {
            var changeCounterLatestCopy = Interlocked.CompareExchange(
                ref _subscriptionsChangeCounter, 0, 0);

            if (_localSubscriptionRevision.Value == changeCounterLatestCopy) { return _localSubscriptions.Value; }

            List<Subscription> latestSubscriptions;
            lock (_allSubscriptions) { latestSubscriptions = _allSubscriptions.ToList(); }

            _localSubscriptionRevision.Value = changeCounterLatestCopy;
            _localSubscriptions.Value = latestSubscriptions;
            return _localSubscriptions.Value;
        }
    }
}
