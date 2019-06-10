﻿namespace Hermit
{
    /// <summary>
    /// Prototype for subscribers action.
    /// </summary>
    public delegate void EventAction();

    /// <summary>
    /// Prototype for subscribers action.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public delegate void EventAction<in T>(T eventData);

    public interface IEventBroker
    {
        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction">Callback.</param>
        void Subscribe<T>(string channel, EventAction<T> eventAction);

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction"></param>
        void Subscribe(string channel, EventAction eventAction);

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        void Unsubscribe<T>(string channel, EventAction<T> eventAction, bool keepEvent = false);

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        void Unsubscribe(string channel, EventAction eventAction, bool keepEvent = false);

        /// <summary>
        /// Unsubscribe all callbacks from event.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        void UnsubscribeAll(string channel, bool keepEvent = false);

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventMessage">Event message.</param>
        void Publish<T>(string channel, T eventMessage);

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="channel"></param>
        void Publish(string channel);
    }
}