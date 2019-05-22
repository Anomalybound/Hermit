using UnityEngine;

namespace Hermit.Services
{
    public class UnityTime : ITime
    {
        public float LogicTime => Time.time;

        public float RealTime => Time.realtimeSinceStartup;
    }
}