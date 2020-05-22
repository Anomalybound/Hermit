using System;
using UnityEngine.Events;

namespace Hermit.Common.DataBinding.Core
{
    public class UnityEventBinder : EventBinderBase
    {
        public UnityEvent UnityEvent { get; }

        public Action Action { get; }

        public UnityEventBinder(UnityEvent unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public UnityEventBinder(UnityEvent unityEvent, Delegate action)
        {
            UnityEvent = unityEvent;
            Action = action as Action;

            if (Action == null)
            {
                throw new InvalidCastException($"{action} is not a suitable parameter for {typeof(UnityEventBinder)}");
            }
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

        public Action Action { get; }

        public Action<T> GenericAction { get; }

        public bool MatchArguments { get; }

        public UnityEventBinder(UnityEvent<T> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public UnityEventBinder(UnityEvent<T> unityEvent, Delegate action)
        {
            UnityEvent = unityEvent;
            GenericAction = action as Action<T>;
            MatchArguments = true;

            if (GenericAction == null)
            {
                throw new InvalidCastException($"{action} is not a suitable parameter for {typeof(UnityEventBinder)}");
            }
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
            if (MatchArguments) { GenericAction?.Invoke(arg0); }
            else { Action?.Invoke(); }
        }
    }

    public class UnityEventBinder<T0, T1> : EventBinderBase
    {
        public UnityEvent<T0, T1> UnityEvent { get; }

        public Action Action { get; }

        public Action<T0, T1> GenericAction { get; }

        public bool MatchArguments { get; }

        public UnityEventBinder(UnityEvent<T0, T1> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public UnityEventBinder(UnityEvent<T0, T1> unityEvent, Delegate action)
        {
            UnityEvent = unityEvent;
            GenericAction = action as Action<T0, T1>;
            MatchArguments = true;

            if (GenericAction == null)
            {
                throw new InvalidCastException($"{action} is not a suitable parameter for {typeof(UnityEventBinder)}");
            }
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
            if (MatchArguments) { GenericAction?.Invoke(arg0, arg1); }
            else { Action?.Invoke(); }
        }
    }

    public class UnityEventBinder<T0, T1, T2> : EventBinderBase
    {
        public UnityEvent<T0, T1, T2> UnityEvent { get; }

        public Action Action { get; }

        public Action<T0, T1, T2> GenericAction { get; }

        public bool MatchArguments { get; }

        public UnityEventBinder(UnityEvent<T0, T1, T2> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public UnityEventBinder(UnityEvent<T0, T1, T2> unityEvent, Delegate action)
        {
            UnityEvent = unityEvent;
            GenericAction = action as Action<T0, T1, T2>;
            MatchArguments = true;

            if (GenericAction == null)
            {
                throw new InvalidCastException($"{action} is not a suitable parameter for {typeof(UnityEventBinder)}");
            }
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
            if (MatchArguments) { GenericAction?.Invoke(arg0, arg1, arg2); }
            else { Action?.Invoke(); }
        }
    }

    public class UnityEventBinder<T0, T1, T2, T3> : EventBinderBase
    {
        public UnityEvent<T0, T1, T2, T3> UnityEvent { get; }

        public Action Action { get; }

        public Action<T0, T1, T2, T3> GenericAction { get; }

        public bool MatchArguments { get; }

        public UnityEventBinder(UnityEvent<T0, T1, T2, T3> unityEvent, Action action)
        {
            UnityEvent = unityEvent;
            Action = action;
        }

        public UnityEventBinder(UnityEvent<T0, T1, T2, T3> unityEvent, Delegate action)
        {
            UnityEvent = unityEvent;
            GenericAction = action as Action<T0, T1, T2, T3>;
            MatchArguments = true;

            if (GenericAction == null)
            {
                throw new InvalidCastException($"{action} is not a suitable parameter for {typeof(UnityEventBinder)}");
            }
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
            if (MatchArguments) { GenericAction?.Invoke(arg0, arg1, arg2, arg3); }
            else { Action?.Invoke(); }
        }
    }
}