using System;
using Hermit.Service.Messages;

namespace Hermit
{
    public partial class Her
    {
        public static IMessageHub CreateMessageHub()
        {
            return new MessageHub();
        }

        public static void Publish<T>(T payloads)
        {
            Current.MessageHub.Publish(payloads);
        }

        public static Guid Subscribe<T>(Action<T> onMessage)
        {
            return Current.MessageHub.Subscribe(onMessage);
        }

        public static Guid Subscribe<T>(Action<T> action, TimeSpan limitation)
        {
            return Current.MessageHub.Subscribe(action, limitation);
        }

        public static void Unsubscribe(Guid token)
        {
            Current.MessageHub.Unsubscribe(token);
        }

        public static bool IsSubscribed(Guid token)
        {
            return Current.MessageHub.IsSubscribed(token);
        }

        public static void ClearAllSubscriptions()
        {
            Current.MessageHub.ClearSubscriptions();
        }

        public static void RegisterGlobalHandler(Action<Type, object> onMessage)
        {
            Current.MessageHub.RegisterGlobalHandler(onMessage);
        }

        public static void RegisterGlobalErrorHandler(Action<Guid, Exception> onError)
        {
            Current.MessageHub.RegisterGlobalErrorHandler(onError);
        }
    }
}
