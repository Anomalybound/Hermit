using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;

namespace Hermit.DataBinding
{
    public abstract class EventBinderBase : IEventBinder
    {
        public abstract void Connect();

        public abstract void Disconnect();

        public static IEventBinder CreateEventBinder(object target, EventInfo eventInfo, Delegate action)
        {
            var handlerType = eventInfo.EventHandlerType;
            var invokeMethod = handlerType.GetMethod("Invoke");
            var valueArguments = invokeMethod?.GetParameters().Select(p => p.ParameterType).ToArray();

            if (valueArguments == null) { return null; }

            var eventBinderType = valueArguments.Length <= 0
                ? typeof(EventBinder)
                : typeof(EventBinder<>).MakeGenericType(valueArguments);

            return Activator.CreateInstance(eventBinderType, eventInfo, target, action) as IEventBinder;
        }

        public static IEventBinder CreateUnityEventBinder(object eventInstance, Type eventType, Action action)
        {
            var unityEventType = GetUnityEventType(eventType);
            if (unityEventType == null) { throw new Exception($"Event {eventInstance} is not an UnityEvent."); }

            var valueArguments = unityEventType.GetGenericArguments();
            var unityEventBinderType = valueArguments.Length <= 0
                ? typeof(UnityEventBinder)
                : typeof(UnityEventBinder<>).MakeGenericType(valueArguments);

            return Activator.CreateInstance(unityEventBinderType, eventInstance, action) as IEventBinder;
        }

        public static IEventBinder CreateUnityEventBinder(object eventInstance, Type eventType, Delegate action)
        {
            var unityEventType = GetUnityEventType(eventType);
            if (unityEventType == null) { throw new Exception($"Event {eventInstance} is not an UnityEvent."); }

            var valueArguments = unityEventType.GetGenericArguments();
            var unityEventBinderType = valueArguments.Length <= 0
                ? typeof(UnityEventBinder)
                : typeof(UnityEventBinder<>).MakeGenericType(valueArguments);

            return Activator.CreateInstance(unityEventBinderType, eventInstance, action) as IEventBinder;
        }

        public static Type GetUnityEventType(Type type)
        {
            var ret = type;
            while (ret.BaseType != null)
            {
                if (ret.BaseType == typeof(UnityEventBase)) { return ret; }

                ret = ret.BaseType;
            }

            return ret;
        }
    }
}