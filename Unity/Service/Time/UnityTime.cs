using Hermit.Common;

namespace Hermit.Service.Time
{
    public class UnityTime : Singleton<UnityTime>, ITime
    {
        public float LogicTime => UnityEngine.Time.time;

        public float RealTime => UnityEngine.Time.realtimeSinceStartup;
    }
}