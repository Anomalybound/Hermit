namespace Hermit
{
    public partial class Her
    {
        #region Events

        public static void On(string eventName, EventAction action)
        {
            Current._eventBroker.Subscribe(eventName, action);
        }

        public static void On<T>(string eventName, EventAction<T> action)
        {
            Current._eventBroker.Subscribe(eventName, action);
        }

        public static void Un(string eventName, EventAction action, bool keepEvent = false)
        {
            Current._eventBroker.Unsubscribe(eventName, action, keepEvent);
        }

        public static void Un<T>(string eventName, EventAction<T> action, bool keepEvent = false)
        {
            Current._eventBroker.Unsubscribe(eventName, action, keepEvent);
        }

        public static void Fire(string eventName)
        {
            Current._eventBroker.Publish(eventName);
        }

        public static void Fire<T>(string eventName, T message)
        {
            Current._eventBroker.Publish(eventName, message);
        }

        #endregion
    }
}