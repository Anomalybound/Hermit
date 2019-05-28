using System;
using System.Reflection;
using UnityEngine.Assertions;

namespace Hermit.DataBinding
{
    public class EventBinder : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        protected readonly Action EventHandler;

        public EventBinder(EventInfo eventInfo, object target, Action action)
        {
            EventInfo = eventInfo;
            Target = target;
            Action = action;

            EventHandler = Action;
        }

        public override void Connect()
        {
            Assert.IsNull(Target);
            EventInfo.AddEventHandler(Target, EventHandler);
        }

        public override void Disconnect()
        {
            Assert.IsNull(Target);
            EventInfo.RemoveEventHandler(Target, EventHandler);
        }
    }

    public class EventBinder<T0> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        protected readonly Action<T0> EventHandler;

        public EventBinder(EventInfo eventInfo, object target, Action action)
        {
            EventInfo = eventInfo;
            Target = target;
            Action = action;

            EventHandler = arg1 => Action();
        }

        public override void Connect()
        {
            Assert.IsNotNull(Target);
            EventInfo.AddEventHandler(Target, EventHandler);
        }

        public override void Disconnect()
        {
            Assert.IsNotNull(Target);
            EventInfo.RemoveEventHandler(Target, EventHandler);
        }
    }

    public class EventBinder<T0, T1> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        protected readonly Action<T0, T1> EventHandler;

        public EventBinder(EventInfo eventInfo, object target, Action action)
        {
            EventInfo = eventInfo;
            Target = target;
            Action = action;

            EventHandler = (arg1, arg2) => Action();
        }

        public override void Connect()
        {
            Assert.IsNotNull(Target);
            EventInfo.AddEventHandler(Target, EventHandler);
        }

        public override void Disconnect()
        {
            Assert.IsNotNull(Target);
            EventInfo.RemoveEventHandler(Target, EventHandler);
        }
    }

    public class EventBinder<T0, T1, T2> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        protected readonly Action<T0, T1, T2> EventHandler;

        public EventBinder(EventInfo eventInfo, object target, Action action)
        {
            EventInfo = eventInfo;
            Target = target;
            Action = action;

            EventHandler = (arg1, arg2, arg3) => Action();
        }

        public override void Connect()
        {
            Assert.IsNotNull(Target);
            EventInfo.AddEventHandler(Target, EventHandler);
        }

        public override void Disconnect()
        {
            Assert.IsNotNull(Target);
            EventInfo.RemoveEventHandler(Target, EventHandler);
        }
    }

    public class EventBinder<T0, T1, T2, T3> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        protected readonly Action<T0, T1, T2, T3> EventHandler;

        public EventBinder(EventInfo eventInfo, object target, Action action)
        {
            EventInfo = eventInfo;
            Target = target;
            Action = action;

            EventHandler = (arg1, arg2, arg3, arg4) => Action();
        }

        public override void Connect()
        {
            Assert.IsNotNull(Target);
            EventInfo.AddEventHandler(Target, EventHandler);
        }

        public override void Disconnect()
        {
            Assert.IsNotNull(Target);
            EventInfo.RemoveEventHandler(Target, EventHandler);
        }
    }

    public class EventBinder<T0, T1, T2, T3, T4> : EventBinderBase
    {
        public EventInfo EventInfo { get; }

        protected object Target { get; }

        protected readonly Action<T0, T1, T2, T3, T4> EventHandler;

        public EventBinder(EventInfo eventInfo, object target, Action action)
        {
            EventInfo = eventInfo;
            Target = target;
            Action = action;

            EventHandler = (arg1, arg2, arg3, arg4, arg5) => Action();
        }

        public override void Connect()
        {
            Assert.IsNotNull(Target);
            EventInfo.AddEventHandler(Target, EventHandler);
        }

        public override void Disconnect()
        {
            Assert.IsNotNull(Target);
            EventInfo.RemoveEventHandler(Target, EventHandler);
        }
    }
}