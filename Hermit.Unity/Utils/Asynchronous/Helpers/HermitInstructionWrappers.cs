using System.Collections;
using UnityEngine.Networking;

namespace Hermit.Async
{
    public static class HermitInstructionWrappers
    {
        public static IEnumerator UnityWebRequest(
            IEnumeratorAwaitExtensions.SimpleCoroutineAwaiter<UnityWebRequest> awaiter,
            UnityWebRequest instruction)
        {
            yield return instruction.SendWebRequest();
            awaiter.Complete(instruction, null);
        }
    }
}