using Hermit.Utils;
using UnityEngine;

namespace Hermit
{
    public static class UnityAwaiterExtensions
    {
        #region IEnumerator

        #endregion

        #region Misc

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