using System;
using System.Reflection;

namespace Hermit.DataBinding
{
    public class EventBinder : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        public Action EventHandler { get; }

        public EventBinder(EventInfo eventInfo, object target, Action eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            EventHandler = eventHandler;
            if (EventHandler == null) { throw new InvalidCastException(); }
        }

        public override void Connect()
        {
            if (EventHandler != null) { EventInfo.AddEventHandler(Target, EventHandler); }
        }

        public override void Disconnect()
        {
            if (EventHandler != null) { EventInfo.RemoveEventHandler(Target, EventHandler); }
        }
    }

    public class EventBinder<T0> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        public Action EventHandler { get; }

        public Action<T0> GenericEventHandler { get; }

        public Delegate ActionDelegate { get; }

        public EventBinder(EventInfo eventInfo, object target, Action eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            EventHandler = eventHandler;
            ActionDelegate = Delegate.CreateDelegate(typeof(Action<T0>), this, nameof(InvokeAction));
        }

        public EventBinder(EventInfo eventInfo, object target, Action<T0> eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            GenericEventHandler = eventHandler;
        }

        public override void Connect()
        {
            if (EventHandler != null) { EventInfo.AddEventHandler(Target, ActionDelegate); }

            if (GenericEventHandler != null) { EventInfo.AddEventHandler(Target, GenericEventHandler); }
        }

        public override void Disconnect()
        {
            if (EventHandler != null) { EventInfo.RemoveEventHandler(Target, ActionDelegate); }

            if (GenericEventHandler != null) { EventInfo.RemoveEventHandler(Target, GenericEventHandler); }
        }

        protected void InvokeAction(T0 arg0)
        {
            EventHandler.Invoke();
        }
    }

    public class EventBinder<T0, T1> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        public Action EventHandler { get; }

        public Action<T0, T1> GenericEventHandler { get; }

        public Delegate ActionDelegate { get; }

        public EventBinder(EventInfo eventInfo, object target, Action eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            EventHandler = eventHandler;
            ActionDelegate = Delegate.CreateDelegate(typeof(Action<T0>), this, nameof(InvokeAction));
        }

        public EventBinder(EventInfo eventInfo, object target, Action<T0, T1> eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            GenericEventHandler = eventHandler;
        }

        public override void Connect()
        {
            if (EventHandler != null) { EventInfo.AddEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.AddEventHandler(Target, GenericEventHandler); }
        }

        public override void Disconnect()
        {
            if (EventHandler != null) { EventInfo.RemoveEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.RemoveEventHandler(Target, GenericEventHandler); }
        }

        protected void InvokeAction(T0 arg0)
        {
            EventHandler.Invoke();
        }
    }

    public class EventBinder<T0, T1, T2> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        public Action EventHandler { get; }

        protected readonly Action<T0, T1, T2> GenericEventHandler;

        public Delegate ActionDelegate { get; }

        public EventBinder(EventInfo eventInfo, object target, Action eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            EventHandler = eventHandler;
            ActionDelegate = Delegate.CreateDelegate(typeof(Action<T0>), this, nameof(InvokeAction));
        }

        public EventBinder(EventInfo eventInfo, object target, Action<T0, T1, T2> eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            GenericEventHandler = eventHandler;
        }

        public override void Connect()
        {
            if (EventHandler != null) { EventInfo.AddEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.AddEventHandler(Target, GenericEventHandler); }
        }

        public override void Disconnect()
        {
            if (EventHandler != null) { EventInfo.RemoveEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.RemoveEventHandler(Target, GenericEventHandler); }
        }

        protected void InvokeAction(T0 arg0)
        {
            EventHandler.Invoke();
        }
    }

    public class EventBinder<T0, T1, T2, T3> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        public Action EventHandler { get; }

        protected readonly Action<T0, T1, T2, T3> GenericEventHandler;

        public Delegate ActionDelegate { get; }

        public EventBinder(EventInfo eventInfo, object target, Action eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            EventHandler = eventHandler;
            ActionDelegate = Delegate.CreateDelegate(typeof(Action<T0>), this, nameof(InvokeAction));
        }

        public EventBinder(EventInfo eventInfo, object target, Action<T0, T1, T2, T3> eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            GenericEventHandler = eventHandler;
        }

        public override void Connect()
        {
            if (EventHandler != null) { EventInfo.AddEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.AddEventHandler(Target, GenericEventHandler); }
        }

        public override void Disconnect()
        {
            if (EventHandler != null) { EventInfo.RemoveEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.RemoveEventHandler(Target, GenericEventHandler); }
        }

        protected void InvokeAction(T0 arg0)
        {
            EventHandler.Invoke();
        }
    }

    public class EventBinder<T0, T1, T2, T3, T4> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        public Action EventHandler { get; }

        protected readonly Action<T0, T1, T2, T3, T4> GenericEventHandler;

        public Delegate ActionDelegate { get; }

        public EventBinder(EventInfo eventInfo, object target, Action eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            EventHandler = eventHandler;
            ActionDelegate = Delegate.CreateDelegate(typeof(Action<T0>), this, nameof(InvokeAction));
        }

        public EventBinder(EventInfo eventInfo, object target, Action<T0, T1, T2, T3, T4> eventHandler)
        {
            EventInfo = eventInfo;
            Target = target;

            GenericEventHandler = eventHandler;
        }

        public override void Connect()
        {
            if (EventHandler != null) { EventInfo.AddEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.AddEventHandler(Target, GenericEventHandler); }
        }

        public override void Disconnect()
        {
            if (EventHandler != null) { EventInfo.RemoveEventHandler(Target, EventHandler); }

            if (GenericEventHandler != null) { EventInfo.RemoveEventHandler(Target, GenericEventHandler); }
        }

        protected void InvokeAction(T0 arg0)
        {
            EventHandler.Invoke();
        }
    }
}