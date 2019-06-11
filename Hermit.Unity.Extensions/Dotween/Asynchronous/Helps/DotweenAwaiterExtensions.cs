#if HERMIT_DOTWEEN
using Hermit.Utils;
using UnityEngine;

namespace Hermit
{
    public static class DotweenAwaiterExtensions
    {
        public static TweenAwaiter GetAwaiter(this DG.Tweening.Tween self)
        {
            return new TweenAwaiter(self);
        }

        public static TweenAwaiter GetCancellableAwaiter(this DG.Tweening.Tween self,
            System.Threading.CancellationToken cancellationToken = default)
        {
            return new TweenAwaiter(self, cancellationToken);
        }
    }
}
#endif