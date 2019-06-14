using System;

namespace Hermit
{
    public interface IEventBroker
    {
        #region No channel specified

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="eventAction"></param>
        /// <typeparam name="T"></typeparam>
        void Subscribe<T>(Action<T> eventAction);

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="eventAction"></param>
        /// <typeparam name="T"></typeparam>
        void UnSubscribe<T>(Action<T> eventAction);

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        void Unsubscribe<T>(Action<T> eventAction, bool keepEvent = false);

        /// <summary>
        /// Unsubscribe all callbacks from event.
        /// </summary>
        /// <param name="keepEvent"></param>
        void UnsubscribeAll(bool keepEvent = false);

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="eventMessage">Event message.</param>
        void Publish<T>(T eventMessage);

        #endregion

        #region Channel specified

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction">Callback.</param>
        void Subscribe<T>(string channel, Action<T> eventAction);

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction"></param>
        void Subscribe(string channel, Action eventAction);

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        void Unsubscribe<T>(string channel, Action<T> eventAction, bool keepEvent = false);

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        void Unsubscribe(string channel, Action eventAction, bool keepEvent = false);

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

        #endregion
    }
}