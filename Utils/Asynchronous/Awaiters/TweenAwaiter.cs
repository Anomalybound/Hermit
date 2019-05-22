#if HERMIT_DOTWEEN

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DG.Tweening;

namespace Hermit.Utils
{
    public struct TweenAwaiter : ICriticalNotifyCompletion
    {
        private readonly Tween _tween;
        private CancellationToken _cancellationToken;

        public TweenAwaiter(Tween tween)
        {
            _tween = tween;
        }

        public TweenAwaiter(Tween tween, CancellationToken cancellationToken)
        {
            _tween = tween;
            _cancellationToken = cancellationToken;
        }

        public bool IsCompleted => !_tween.IsPlaying();

        public void GetResult()
        {
            if (_cancellationToken.IsCancellationRequested) { _cancellationToken.ThrowIfCancellationRequested(); }
        }

        public void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        public void UnsafeOnCompleted(Action continuation)
        {
            var tween = _tween;
            var registration = _cancellationToken.Register(() => { tween.Kill(true); });

            tween.OnKill(() =>
            {
                registration.Dispose();
                continuation.Invoke();
            });
        }

        public TweenAwaiter GetAwaiter() => this;
    }
}
#endif