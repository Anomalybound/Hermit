using Hermit.Pool;

namespace Hermit.Common
{
    public static class HermitEvent
    {
        // Register services
        public const string ServiceInjectionStarted = "Hermit.Service.RegisterStarted";
        public const string ServiceInjectionFinished = "Hermit.Service.InjectionFinished";

        // Inject services
        public const string ServiceRegisterStarted = "Hermit.Service.RegisterStarted";
        public const string ServiceRegisterFinished = "Hermit.Service.RegisterFinished";
        
        // Procedure
        public const string ProcedureBuildStateStarted = "Hermit.Procedure.BuildStateStarted";
        public const string ProcedureBuildStateFinished = "Hermit.Procedure.BuildStateFinished";
        
        internal static void Send(string endpoint, string msg = null)
        {
            var eventData = EventPool<SystemEvent>.Rent();
            eventData.Message = msg;
            Her.Trigger(endpoint, eventData);
        }
    }
}