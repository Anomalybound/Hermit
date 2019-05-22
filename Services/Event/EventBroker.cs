using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hermit.Services
{
    /// <summary>
    /// IEventBroker implementation.
    /// </summary>
    public sealed class EventBroker : IEventBroker
    {
        private const int MaxCallDepth = 5;

        private int _eventsInCall;

        private readonly Dictionary<string, Dictionary<Type, Delegate>> _events =
            new Dictionary<string, Dictionary<Type, Delegate>>(32);

        public static EventBroker Current => new EventBroker();

        public void Subscribe<T>(string eventName, EventAction<T> eventAction)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(T);

            if (!_events.TryGetValue(eventName, out var delegates))
            {
                var action = eventAction;
                delegates = new Dictionary<Type, Delegate> {{eventType, action}};
                _events.Add(eventName, delegates);
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

        public void Subscribe(string eventName, EventAction eventAction)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(EventAction);

            if (!_events.TryGetValue(eventName, out var delegates))
            {
                var action = eventAction;
                delegates = new Dictionary<Type, Delegate> {{eventType, action}};
                _events.Add(eventName, delegates);
            }
            else
            {
                if (delegates.TryGetValue(eventType, out var del))
                {
                    delegates[eventType] = (del as EventAction) + eventAction;
                }
                else
                {
                    var action = eventAction;
                    delegates.Add(eventType, action);
                }
            }
        }

        public void Unsubscribe<T>(string eventName, EventAction<T> eventAction, bool keepEvent = false)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(T);

            if (!_events.TryGetValue(eventName, out var delegates)) { return; }

            if (!delegates.TryGetValue(eventType, out var del)) { return; }

            if (del == null) { return; }

            var ret = (EventAction<T>) del - eventAction;
            if (ret == null && !keepEvent)
            {
                delegates.Remove(eventType);
                if (delegates.Count <= 0) { _events.Remove(eventName); }
            }
            else { delegates[eventType] = ret; }
        }

        public void Unsubscribe(string eventName, EventAction eventAction, bool keepEvent = false)
        {
            if (eventAction == null) { throw new Exception("No subscriber."); }

            var eventType = typeof(EventAction);

            if (!_events.TryGetValue(eventName, out var delegates)) { return; }

            if (!delegates.TryGetValue(eventType, out var del)) { return; }

            if (del == null) { return; }

            var ret = (EventAction) del - eventAction;
            if (ret == null && !keepEvent)
            {
                delegates.Remove(eventType);
                if (delegates.Count <= 0) { _events.Remove(eventName); }
            }
            else { delegates[eventType] = ret; }
        }

        public void UnsubscribeAll(string eventName, bool keepEvent = false)
        {
            if (!_events.TryGetValue(eventName, out var delegates)) { return; }

            if (keepEvent)
            {
                foreach (var valuePair in delegates) { delegates[valuePair.Key] = null; }
            }
            else { _events[eventName].Clear(); }
        }

        public void Publish<T>(string eventName, T eventMessage)
        {
            if (_eventsInCall >= MaxCallDepth)
            {
                Debug.LogError("Max call depth reached");
                return;
            }

            if (eventMessage == null) { throw new Exception("Message is null."); }

            var eventType = eventMessage.GetType();

            if (!_events.TryGetValue(eventName, out var delegates)) { return; }

            if (!delegates.TryGetValue(eventType, out var del)) { return; }

            var evtAction = del as EventAction<T>;

            _eventsInCall++;
            try { evtAction?.Invoke(eventMessage); }
            catch (Exception ex) { Debug.LogException(ex); }

            _eventsInCall--;
        }

        public void Publish(string eventName)
        {
            if (_eventsInCall >= MaxCallDepth)
            {
                Debug.LogError("Max call depth reached");
                return;
            }

            var eventType = typeof(EventAction);

            if (!_events.TryGetValue(eventName, out var delegates)) { return; }

            if (!delegates.TryGetValue(eventType, out var del)) { return; }

            var evtAction = del as EventAction;

            _eventsInCall++;
            try { evtAction?.Invoke(); }
            catch (Exception ex) { Debug.LogException(ex); }

            _eventsInCall--;
        }
    }
}