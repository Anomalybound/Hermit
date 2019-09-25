using System;

namespace Hermit
{
    /// <summary>
    /// Her.Event would be available anytime.
    /// </summary>
    public partial class Her
    {
        #region Events

        #region No channel

        public static void Subscribe<T>(Action<T> action)
        {
            Current.EventBroker.Subscribe(action);
        }

        public static void UnSubscribe<T>(Action<T> action, bool keepEvent = false)
        {
            Current.EventBroker.Unsubscribe(action, keepEvent);
        }

        public static void UnSubscribeAll(bool keepEvent = false)
        {
            Current.EventBroker.UnsubscribeAll(keepEvent);
        }

        public static void Publish<T>(T message)
        {
            Current.EventBroker.Publish(message);
        }

        #endregion

        #region With Channel

        public static void Subscribe(string channel, Action action)
        {
            Current.EventBroker.Subscribe(channel, action);
        }

        public static void Subscribe<T>(string channel, Action<T> action)
        {
            Current.EventBroker.Subscribe(channel, action);
        }

        public static void UnSubscribe(string channel, Action action, bool keepEvent = false)
        {
            Current.EventBroker.Unsubscribe(channel, action, keepEvent);
        }

        public static void UnSubscribe<T>(string channel, Action<T> action, bool keepEvent = false)
        {
            Current.EventBroker.Unsubscribe(channel, action, keepEvent);
        }

        public static void Publish(string channel)
        {
            Current.EventBroker.Publish(channel);
        }

        public static void Publish<T>(string channel, T message)
        {
            Current.EventBroker.Publish(channel, message);
        }

        public static void UnSubscribeAll(string channel, bool keepEvent = false)
        {
            Current.EventBroker.UnsubscribeAll(channel, keepEvent);
        }

        #endregion

        #endregion
    }
}