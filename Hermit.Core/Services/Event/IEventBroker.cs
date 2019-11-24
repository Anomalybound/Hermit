using System;
using System.Threading.Tasks;

namespace Hermit
{
    public interface IEventBroker
    {
        #region Register

        void Register(object obj);

        void Unregister(object obj);

        #endregion

        #region Trigger

        void Trigger(string endpoint, bool sticky = false);

        void Trigger<TEventData>(TEventData payloads, bool sticky = false)
            where TEventData : EventData;

        void Trigger<TEventData>(string endpoint, TEventData payloads, bool sticky = false)
            where TEventData : EventData;

        Task TriggerAsync(string endpoint);

        Task TriggerAsync<TEventData>(TEventData payloads)
            where TEventData : EventData;

        Task TriggerAsync<TEventData>(string endpoint, TEventData payloads)
            where TEventData : EventData;

        #endregion
    }
}