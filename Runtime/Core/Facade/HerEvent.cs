using Cysharp.Threading.Tasks;

namespace Hermit
{
    public partial class Her
    {
        #region Registeration

        public static void Listen(object obj)
        {
            I.EventBroker.Register(obj);
        }

        public static void UnListen(object obj)
        {
            I.EventBroker.Unregister(obj);
        }

        #endregion

        #region Trigger

        public static void Trigger(string endpoint)
        {
            I.EventBroker.Trigger(endpoint);
        }

        public static void Trigger<TEventData>(TEventData payloads) where TEventData : Payloads
        {
            I.EventBroker.Trigger(payloads);
        }

        public static void TriggerSticky<TEventData>(string endpoint, TEventData payloads) where TEventData : Payloads
        {
            I.EventBroker.TriggerSticky(endpoint, payloads);
        }

        public static void TriggerSticky(string endpoint)
        {
            I.EventBroker.TriggerSticky(endpoint);
        }

        public static void TriggerSticky<TEventData>(TEventData payloads) where TEventData : Payloads
        {
            I.EventBroker.TriggerSticky(payloads);
        }

        public static void Trigger<TEventData>(string endpoint, TEventData payloads) where TEventData : Payloads
        {
            I.EventBroker.Trigger(endpoint, payloads);
        }

        public static UniTask TriggerAsync(string endpoint)
        {
            return I.EventBroker.TriggerAsync(endpoint);
        }

        public static UniTask TriggerAsync<TEventData>(TEventData payloads)
            where TEventData : Payloads
        {
            return I.EventBroker.TriggerAsync(payloads);
        }

        public static UniTask TriggerAsync<TEventData>(string endpoint, TEventData payloads)
            where TEventData : Payloads
        {
            return I.EventBroker.TriggerAsync(endpoint, payloads);
        }

        #endregion
    }
}
