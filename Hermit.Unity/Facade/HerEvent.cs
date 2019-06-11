namespace Hermit
{
    public partial class Her
    {
        #region Events

        public static void On(string channel, EventAction action)
        {
            Current.eventBroker.Subscribe(channel, action);
        }

        public static void On<T>(string channel, EventAction<T> action)
        {
            Current.eventBroker.Subscribe(channel, action);
        }

        public static void Un(string channel, EventAction action, bool keepEvent = false)
        {
            Current.eventBroker.Unsubscribe(channel, action, keepEvent);
        }

        public static void Un<T>(string channel, EventAction<T> action, bool keepEvent = false)
        {
            Current.eventBroker.Unsubscribe(channel, action, keepEvent);
        }

        public static void Fire(string channel)
        {
            Current.eventBroker.Publish(channel);
        }

        public static void Fire<T>(string channel, T message)
        {
            Current.eventBroker.Publish(channel, message);
        }

        #endregion
    }
}