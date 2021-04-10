using System;
using Hermit.Service.Messages;

namespace Hermit
{
    public partial class App
    {
        public static void Publish<T>(T payloads)
        {
            I.MessageHub.Publish(payloads);
        }

        public static Guid Subscribe<T>(Action<T> onMessage)
        {
            return I.MessageHub.Subscribe(onMessage);
        }

        public static Guid Subscribe<T>(Action<T> action, TimeSpan limitation)
        {
            return I.MessageHub.Subscribe(action, limitation);
        }

        public static void Unsubscribe(Guid token)
        {
            I.MessageHub.Unsubscribe(token);
        }

        public static bool IsSubscribed(Guid token)
        {
            return I.MessageHub.IsSubscribed(token);
        }

        public static void ClearAllSubscriptions()
        {
            I.MessageHub.ClearSubscriptions();
        }

        public static void RegisterGlobalHandler(Action<Type, object> onMessage)
        {
            I.MessageHub.RegisterGlobalHandler(onMessage);
        }

        public static void RegisterGlobalErrorHandler(Action<Guid, Exception> onError)
        {
            I.MessageHub.RegisterGlobalErrorHandler(onError);
        }
    }
}
