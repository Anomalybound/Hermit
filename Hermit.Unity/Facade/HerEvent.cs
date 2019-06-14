using System;

namespace Hermit
{
    public partial class Her
    {
        #region Events

        #region No channel

        public static void Subscribe<T>(Action<T> action)
        {
            Current.eventBroker.Subscribe(action);
        }

        public static void UnSubscribe<T>(Action<T> action, bool keepEvent = false)
        {
            Current.eventBroker.Unsubscribe(action, keepEvent);
        }

        public static void UnSubscribeAll(bool keepEvent = false)
        {
            Current.eventBroker.UnsubscribeAll(keepEvent);
        }

        public static void Publish<T>(T message)
        {
            Current.eventBroker.Publish(message);
        }

        #endregion

        #region With Channel

        public static void Subscribe(string channel, Action action)
        {
            Current.eventBroker.Subscribe(channel, action);
        }

        public static void Subscribe<T>(string channel, Action<T> action)
        {
            Current.eventBroker.Subscribe(channel, action);
        }

        public static void UnSubscribe(string channel, Action action, bool keepEvent = false)
        {
            Current.eventBroker.Unsubscribe(channel, action, keepEvent);
        }

        public static void UnSubscribe<T>(string channel, Action<T> action, bool keepEvent = false)
        {
            Current.eventBroker.Unsubscribe(channel, action, keepEvent);
        }

        public static void Publish(string channel)
        {
            Current.eventBroker.Publish(channel);
        }

        public static void Publish<T>(string channel, T message)
        {
            Current.eventBroker.Publish(channel, message);
        }

        public static void UnSubscribeAll(string channel, bool keepEvent = false)
        {
            Current.eventBroker.UnsubscribeAll(channel, keepEvent);
        }

        #endregion

        #endregion
    }
}