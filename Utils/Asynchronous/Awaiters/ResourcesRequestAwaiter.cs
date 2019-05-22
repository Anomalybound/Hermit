using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Hermit.Utils
{
    public struct ResourcesRequestAwaiter : ICriticalNotifyCompletion
    {
        private readonly ResourceRequest _asyncOperation;

        public ResourcesRequestAwaiter(ResourceRequest asyncOperation)
        {
            _asyncOperation = asyncOperation;
        }

        public ResourcesRequestAwaiter GetAwaiter() => this;

        public bool IsCompleted => _asyncOperation.isDone;

        public void OnCompleted(Action action) => UnsafeOnCompleted(action);

        public void UnsafeOnCompleted(Action continuation)
        {
            continuation.Invoke();
        }

        public void GetResult() { }

        private async Task GetValue(Action action)
        {
            while (!_asyncOperation.isDone) { await Task.Yield(); }

            action();
        }
    }
}