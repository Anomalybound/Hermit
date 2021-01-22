using System;
using System.Collections;

namespace Hermit.Service.Events
{
    public interface IDispatcher
    {
        void Dispatch(IEnumerator action);
        
        void Dispatch(Action action);
    }
}
