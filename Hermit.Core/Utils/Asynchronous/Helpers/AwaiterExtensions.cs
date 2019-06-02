using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hermit
{
    public static class AwaiterExtensions
    {
        #region IEnumerator

        #endregion

        #region Misc

        public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
        {
            return Task.Delay(timeSpan).GetAwaiter();
        }

        #endregion
    }
}