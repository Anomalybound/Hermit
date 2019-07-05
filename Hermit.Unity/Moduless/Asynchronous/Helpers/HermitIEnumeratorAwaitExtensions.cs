using System;
using System.Threading;
using Hermit.Async;
using UnityAsyncAwaitUtil;
using UnityEngine.Networking;

public static class HermitIEnumeratorAwaitExtensions
{
    public static IEnumeratorAwaitExtensions.SimpleCoroutineAwaiter<UnityWebRequest> GetAwaiter(
        this UnityWebRequest instruction)
    {
        var awaiter = new IEnumeratorAwaitExtensions.SimpleCoroutineAwaiter<UnityWebRequest>();
        RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
            HermitInstructionWrappers.UnityWebRequest(awaiter, instruction)));
        return awaiter;
    }

    static void RunOnUnityScheduler(Action action)
    {
        if (SynchronizationContext.Current == SyncContextUtil.UnitySynchronizationContext) { action(); }
        else { SyncContextUtil.UnitySynchronizationContext.Post(_ => action(), null); }
    }
}