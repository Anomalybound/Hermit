using System;

namespace Hermit
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SubscribeAttribute : Attribute
    {
        public string Endpoint { get; set; } = EventConstants.DefaultEndpoint;

        public ThreadMode Mode { get; set; } = ThreadMode.Main;

        public bool Sticky { get; set; } = false;

        public short Priority { get; set; } = 0;
    }
}