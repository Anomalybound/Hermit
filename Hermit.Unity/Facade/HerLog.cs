using UnityEngine;

namespace Hermit
{
    /// <summary>
    ///  Her.Log would be available anytime.
    /// </summary>
    public partial class Her
    {
        public static void Log(object message, Object context = null)
        {
            Current.Logger.Log(message, context);
        }

        public static void Warn(object message, Object context = null)
        {
            Current.Logger.Warn(message, context);
        }

        public static void Error(object message, Object context = null)
        {
            Current.Logger.Error(message, context);
        }
    }
}