using Hermit.Injection;
using UnityEngine;

namespace Hermit
{
    public class EngineRunner : MonoBehaviour
    {
        private static EngineRunner instance;

        public static EngineRunner CreateInstance(IDependencyContainer container)
        {
            if (instance != null) { return instance; }

            var go = new GameObject("Engine Runner");
            instance = go.AddComponent<EngineRunner>();

            return instance;
        }
    }
}