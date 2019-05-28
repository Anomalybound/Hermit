using System;

namespace Hermit.DataBinding
{
    public interface IEventBinder
    {
        Action Action { get; }

        void Connect();

        void Disconnect();
    }
}