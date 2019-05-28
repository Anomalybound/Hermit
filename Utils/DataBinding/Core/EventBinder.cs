using System;
using System.Reflection;
using UnityEngine.Assertions;

namespace Hermit.DataBinding
{
    public class EventBinder : IEventBinder
    {
        public EventInfo EventInfo { get; }

        public Action Action { get; }

        protected object Target { get; }

        public EventBinder(EventInfo eventInfo, object target, Action action)
        {
            EventInfo = eventInfo;
            Target = target;
            Action = action;
        }

        public void Connect()
        {
            Assert.IsNull(Target);
            EventInfo.AddEventHandler(Target, Action);
        }

        public void Disconnect()
        {
            Assert.IsNull(Target);
            EventInfo.RemoveEventHandler(Target, Action);
        }
    }
}