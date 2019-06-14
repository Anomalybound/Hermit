namespace Hermit
{
    public partial class Her
    {
        #region Events

        public static void Subscribe<T>(EventAction<T> action)
        {
            Current.eventBroker.Subscribe(action);
        }

        public static void UnSubscribe<T>(EventAction<T> action, bool keepEvent = false)
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

        public static void Subscribe(string channel, EventAction action)
        {
            Current.eventBroker.Subscribe(channel, action);
        }

        public static void Subscribe<T>(string channel, EventAction<T> action)
        {
            Current.eventBroker.Subscribe(channel, action);
        }

        public static void UnSubscribe(string channel, EventAction action, bool keepEvent = false)
        {
            Current.eventBroker.Unsubscribe(channel, action, keepEvent);
        }

        public static void UnSubscribe<T>(string channel, EventAction<T> action, bool keepEvent = false)
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
    }
}