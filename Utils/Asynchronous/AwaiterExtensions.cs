using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Hermit.Utils;

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

        public static ResourcesRequestAwaiter GetAwaiter(this ResourceRequest self)
        {
            return new ResourcesRequestAwaiter(self);
        }

#if HERMIT_DOTWEEN
        public static TweenAwaiter GetAwaiter(this DG.Tweening.Tween self)
        {
            return new TweenAwaiter(self);
        }

        public static TweenAwaiter GetCancellableAwaiter(this DG.Tweening.Tween self,
            System.Threading.CancellationToken cancellationToken = default)
        {
            return new TweenAwaiter(self, cancellationToken);
        }
#endif

        #endregion
    }
}