using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Hermit.Services
{
    public class UnityLog : Singleton<UnityLog>, ILog
    {
        [Conditional("DEBUG")]
        public void Log(object obj, Object context = null)
        {
            Debug.Log(obj, context);
        }

        [Conditional("DEBUG")]
        public void Warn(object warning, Object context = null)
        {
            Debug.LogWarning(warning, context);
        }

        [Conditional("DEBUG")]
        public void Error(object error, Object context = null)
        {
            Debug.LogError(error, context);
        }
    }
}