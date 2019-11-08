using System;
using System.Collections.Generic;

namespace Hermit.Services
{
    /// <summary>
    /// IEventBroker implementation.
    /// </summary>
    public sealed class EventBroker : Singleton<EventBroker>, IEventBroker
    {
        private const int MaxCallDepth = 5;

        private int _eventsInCall;

        private readonly Dictionary<string, Dictionary<Type, Delegate>> _genericEvents =
            new Dictionary<string, Dictionary<Type, Delegate>>(32);

        public const string DefaultChannel = "Default";

        private readonly Dictionary<string, Delegate> _nonGenericEvents = new Dictionary<string, Delegate>(32);

        public void Subscribe<T>(Action<T> eventAction)
        {
            Subscribe(DefaultChannel, eventAction);
        }

        public void UnSubscribe<T>(Action<T> eventAction)
        {
            Unsubscribe(DefaultChannel, eventAction);
        }

        public void Unsubscribe<T>(Action<T> eventAction, bool keepEvent = false)
        {
            Unsubscribe(DefaultChannel, eventAction, keepEvent);
        }

        public void UnsubscribeAll(bool keepEvent = false)
        {
            UnsubscribeAll(DefaultChannel, keepEvent);
        }

        public void Publish<T>(T eventMessage)
        {
            Publish(DefaultChannel, eventMessage);
        }

        public void Subscribe<T>(string channel, Action<T> eventAction)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(T);

            if (!_genericEvents.TryGetValue(channel, out var delegates))
            {
                var action = eventAction;
                delegates = new Dictionary<Type, Delegate> {{eventType, action}};
                _genericEvents.Add(channel, delegates);
            }
            else
            {
                if (delegates.TryGetValue(eventType, out var del))
                {
                    delegates[eventType] = (del as Action<T>) + eventAction;
                }
                else
                {
                    var action = eventAction;
                    delegates.Add(eventType, action);
                }
            }
        }

        public void Subscribe(string channel, Action eventAction)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            if (!_nonGenericEvents.TryGetValue(channel, out var del)) { _nonGenericEvents.Add(channel, eventAction); }
            else { _nonGenericEvents[channel] = (del as Action) + eventAction; }
        }

        public void Unsubscribe<T>(string channel, Action<T> eventAction, bool keepEvent = false)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(T);

            if (!_genericEvents.TryGetValue(channel, out var delegates)) { return; }

            if (!delegates.TryGetValue(eventType, out var del)) { return; }

            if (del == null) { return; }

            var ret = (Action<T>) del - eventAction;
            if (ret == null && !keepEvent)
            {
                delegates.Remove(eventType);
                if (delegates.Count <= 0) { _genericEvents.Remove(channel); }
            }
            else { delegates[eventType] = ret; }
        }

        public void Unsubscribe(string channel, Action eventAction, bool keepEvent = false)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(Action);

            if (!_nonGenericEvents.TryGetValue(channel, out var del)) { return; }

            if (del == null) { return; }

            var ret = (del as Action) - eventAction;
            if (ret == null && !keepEvent) { _nonGenericEvents.Remove(channel); }
            else { _nonGenericEvents[channel] = ret; }
        }

        public void UnsubscribeAll(string channel, bool keepEvent = false)
        {
            if (!_genericEvents.TryGetValue(channel, out var delegates)) { return; }

            if (keepEvent)
            {
                foreach (var valuePair in delegates) { delegates[valuePair.Key] = null; }
            }
            else { _genericEvents[channel].Clear(); }
        }

        public void Publish<T>(string channel, T eventMessage)
        {
            if (_eventsInCall >= MaxCallDepth) { throw new Exception("Max call depth reached"); }

            if (eventMessage == null) { throw new Exception("Message is null."); }

            var eventType = eventMessage.GetType();

            _genericEvents.TryGetValue(channel, out var delegates);

            Delegate genericDelegate = null;

            delegates?.TryGetValue(eventType, out genericDelegate);

            var genericEventAction = genericDelegate as Action<T>;

            _nonGenericEvents.TryGetValue(channel, out genericDelegate);

            var nonGenericEventAction = genericDelegate as Action;

            _eventsInCall++;

            genericEventAction?.Invoke(eventMessage);
            nonGenericEventAction?.Invoke();

            _eventsInCall--;
        }

        public void Publish(string channel)
        {
            if (_eventsInCall >= MaxCallDepth) { throw new Exception("Max call depth reached"); }

            if (!_nonGenericEvents.TryGetValue(channel, out var del)) { return; }

            var evtAction = del as Action;

            _eventsInCall++;

            evtAction?.Invoke();

            _eventsInCall--;
        }
    }
}