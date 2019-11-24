using System.Threading.Tasks;

namespace Hermit
{
    /// <summary>
    /// Her.Event would be available anytime.
    /// </summary>
    public partial class Her
    {
        #region Registeration

        public static void Listen(object obj)
        {
            Current.EventBroker.Register(obj);
        }

        public static void UnListen(object obj)
        {
            Current.EventBroker.Unregister(obj);
        }

        #endregion

        #region Trigger

        public static void Trigger(string endpoint, bool sticky = false)
        {
            Current.EventBroker.Trigger(endpoint, sticky);
        }

        public static void Trigger<TEventData>(TEventData payloads, bool sticky = false)
            where TEventData : EventData
        {
            Current.EventBroker.Trigger(payloads, sticky);
        }

        public static void Trigger<TEventData>(string endpoint, TEventData payloads, bool sticky = false)
            where TEventData : EventData
        {
            Current.EventBroker.Trigger(endpoint, payloads, sticky);
        }

        public static Task TriggerAsync(string endpoint)
        {
            return Current.EventBroker.TriggerAsync(endpoint);
        }

        public static Task TriggerAsync<TEventData>(TEventData payloads)
            where TEventData : EventData
        {
            return Current.EventBroker.TriggerAsync(payloads);
        }

        public static Task TriggerAsync<TEventData>(string endpoint, TEventData payloads)
            where TEventData : EventData
        {
            return Current.EventBroker.TriggerAsync(endpoint, payloads);
        }

        #endregion
    }
}