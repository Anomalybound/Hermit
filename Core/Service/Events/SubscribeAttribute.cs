using System;

namespace Hermit.Service.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SubscribeAttribute : Attribute
    {
        public string Endpoint { get; set; } = EventBroker.DefaultEndpoint;

        public ThreadMode Mode { get; set; } = ThreadMode.Main;

        public bool Sticky { get; set; } = false;

        public short Priority { get; set; } = 0;
    }
}