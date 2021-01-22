using Cysharp.Threading.Tasks;

namespace Hermit.Service.Events
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
            where TEventData : Payloads;

        void Trigger<TEventData>(string endpoint, TEventData payloads)
            where TEventData : Payloads;

        void TriggerSticky(string endpoint);

        void TriggerSticky<TEventData>(TEventData payloads)
            where TEventData : Payloads;

        void TriggerSticky<TEventData>(string endpoint, TEventData payloads)
            where TEventData : Payloads;

        UniTask TriggerAsync(string endpoint);

        UniTask TriggerAsync<TEventData>(TEventData payloads)
            where TEventData : Payloads;

        UniTask TriggerAsync<TEventData>(string endpoint, TEventData payloads)
            where TEventData : Payloads;

        #endregion
    }
}
