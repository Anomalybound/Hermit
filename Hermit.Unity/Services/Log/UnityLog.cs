using Debug = UnityEngine.Debug;

namespace Hermit.Services
{
    public class UnityLog : Singleton<UnityLog>, ILog
    {
        public void Log(object obj)
        {
            Debug.Log(obj);
        }

        public void Warn(object warning)
        {
            Debug.LogWarning(warning);
        }

        public void Error(object error)
        {
            Debug.LogError(error);
        }

        public void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }
    }
}