using UnityEngine;

namespace Hermit
{
    public interface ILog
    {
        void Log(object obj, Object context = null);

        void Warn(object warning, Object context = null);

        void Error(object error, Object context = null);
    }
}