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

        void Trigger(string endpoint);

        void Trigger<TEventData>(TEventData payloads)
            where TEventData : EventData;

        void Trigger<TEventData>(string endpoint, TEventData payloads)
            where TEventData : EventData;


        void TriggerSticky(string endpoint);

        void TriggerSticky<TEventData>(TEventData payloads)
            where TEventData : EventData;

        void TriggerSticky<TEventData>(string endpoint, TEventData payloads)
            where TEventData : EventData;

        Task TriggerAsync(string endpoint);

        Task TriggerAsync<TEventData>(TEventData payloads)
            where TEventData : EventData;

        Task TriggerAsync<TEventData>(string endpoint, TEventData payloads)
            where TEventData : EventData;

        #endregion
    }
}
