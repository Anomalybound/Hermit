using System;
using System.Collections.Generic;

namespace Hermit.Services
{
    /// <summary>
    /// IEventBroker implementation.
    /// </summary>
    public sealed class EventBroker : IEventBroker
    {
        private const int MaxCallDepth = 5;

        private int _eventsInCall;

        private readonly Dictionary<string, Dictionary<Type, Delegate>> _genericEvents =
            new Dictionary<string, Dictionary<Type, Delegate>>(32);

        private readonly Dictionary<string, Delegate> _nonGenericEvents = new Dictionary<string, Delegate>(32);

        public static EventBroker Current => new EventBroker();

        public void Subscribe<T>(EventAction<T> eventAction)
        {
            Subscribe(null, eventAction);
        }

        public void UnSubscribe<T>(EventAction<T> eventAction)
        {
            Unsubscribe(null, eventAction);
        }

        public void UnsubscribeAll(bool keepEvent = false)
        {
            UnsubscribeAll(null, keepEvent);
        }

        public void Publish<T>(T eventMessage)
        {
            Publish(null, eventMessage);
        }

        public void Subscribe<T>(string channel, EventAction<T> eventAction)
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
                    delegates[eventType] = (del as EventAction<T>) + eventAction;
                }
                else
                {
                    var action = eventAction;
                    delegates.Add(eventType, action);
                }
            }
        }

        public void Subscribe(string channel, EventAction eventAction)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            if (!_nonGenericEvents.TryGetValue(channel, out var del)) { _nonGenericEvents.Add(channel, eventAction); }
            else { _nonGenericEvents[channel] = (del as EventAction) + eventAction; }
        }

        public void Unsubscribe<T>(string channel, EventAction<T> eventAction, bool keepEvent = false)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(T);

            if (!_genericEvents.TryGetValue(channel, out var delegates)) { return; }

            if (!delegates.TryGetValue(eventType, out var del)) { return; }

            if (del == null) { return; }

            var ret = (EventAction<T>) del - eventAction;
            if (ret == null && !keepEvent)
            {
                delegates.Remove(eventType);
                if (delegates.Count <= 0) { _genericEvents.Remove(channel); }
            }
            else { delegates[eventType] = ret; }
        }

        public void Unsubscribe(string channel, EventAction eventAction, bool keepEvent = false)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(EventAction);

            if (!_genericEvents.TryGetValue(channel, out var delegates)) { return; }

            if (!delegates.TryGetValue(eventType, out var del)) { return; }

            if (del == null) { return; }

            var ret = (EventAction) del - eventAction;
            if (ret == null && !keepEvent)
            {
                delegates.Remove(eventType);
                if (delegates.Count <= 0) { _genericEvents.Remove(channel); }
            }
            else { delegates[eventType] = ret; }
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

            var genericEventAction = genericDelegate as EventAction<T>;

            _nonGenericEvents.TryGetValue(channel, out genericDelegate);

            var nonGenericEventAction = genericDelegate as EventAction;

            _eventsInCall++;

            try
            {
                genericEventAction?.Invoke(eventMessage);
                nonGenericEventAction?.Invoke();
            }
            catch (Exception ex) { throw ex; }

            _eventsInCall--;
        }

        public void Publish(string channel)
        {
            if (_eventsInCall >= MaxCallDepth) { throw new Exception("Max call depth reached"); }

            if (!_nonGenericEvents.TryGetValue(channel, out var del)) { return; }

            var evtAction = del as EventAction;

            _eventsInCall++;
            try { evtAction?.Invoke(); }
            catch (Exception ex) { throw ex; }

            _eventsInCall--;
        }
    }
}