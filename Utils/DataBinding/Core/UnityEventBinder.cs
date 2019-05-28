using System;
using UnityEngine.Events;

namespace Hermit.DataBinding
{
    public abstract class EventBinderBase : IEventBinder
    {
        public Action Action { get; protected set; }

        public abstract void Connect();

        public abstract void Disconnect();
    }

    public class UnityEventBinder : EventBinderBase
    {
        public UnityEvent UnityEvent { get; }

        public UnityEventBinder(UnityEvent unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public override void Connect()
        {
            UnityEvent.AddListener(OnUnityEventInvoked);
        }

        public override void Disconnect()
        {
            UnityEvent.RemoveListener(OnUnityEventInvoked);
        }

        protected void OnUnityEventInvoked()
        {
            Action?.Invoke();
        }
    }

    public class UnityEventBinder<T> : EventBinderBase
    {
        public UnityEvent<T> UnityEvent { get; }

        public UnityEventBinder(UnityEvent<T> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public override void Connect()
        {
            UnityEvent.AddListener(OnUnityEventInvoked);
        }

        public override void Disconnect()
        {
            UnityEvent.RemoveListener(OnUnityEventInvoked);
        }

        protected void OnUnityEventInvoked(T arg0)
        {
            Action?.Invoke();
        }
    }

    public class UnityEventBinder<T0, T1> : EventBinderBase
    {
        public UnityEvent<T0, T1> UnityEvent { get; }

        public UnityEventBinder(UnityEvent<T0, T1> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public override void Connect()
        {
            UnityEvent.AddListener(OnUnityEventInvoked);
        }

        public override void Disconnect()
        {
            UnityEvent.RemoveListener(OnUnityEventInvoked);
        }

        private void OnUnityEventInvoked(T0 arg0, T1 arg1)
        {
            Action?.Invoke();
        }
    }

    public class UnityEventBinder<T0, T1, T2> : EventBinderBase
    {
        public UnityEvent<T0, T1, T2> UnityEvent { get; }

        public UnityEventBinder(UnityEvent<T0, T1, T2> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public override void Connect()
        {
            UnityEvent.AddListener(OnUnityEventInvoked);
        }

        public override void Disconnect()
        {
            UnityEvent.RemoveListener(OnUnityEventInvoked);
        }

        private void OnUnityEventInvoked(T0 arg0, T1 arg1, T2 arg2)
        {
            Action?.Invoke();
        }
    }

    public class UnityEventBinder<T0, T1, T2, T3> : EventBinderBase
    {
        public UnityEvent<T0, T1, T2, T3> UnityEvent { get; }

        public UnityEventBinder(UnityEvent<T0, T1, T2, T3> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public override void Connect()
        {
            UnityEvent.AddListener(OnUnityEventInvoked);
        }

        public override void Disconnect()
        {
            UnityEvent.RemoveListener(OnUnityEventInvoked);
        }

        private void OnUnityEventInvoked(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            Action?.Invoke();
        }
    }
}