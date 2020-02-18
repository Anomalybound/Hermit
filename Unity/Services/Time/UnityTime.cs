using UnityEngine;

namespace Hermit.Services
{
    public class UnityTime : Singleton<UnityTime>, ITime
    {
        public float LogicTime => Time.time;

        public float RealTime => Time.realtimeSinceStartup;
    }
}