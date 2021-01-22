using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Hermit.Service.Events
{
    public class MainThreadDispatcher : MonoBehaviour, IDispatcher
    {
        private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();

        private static MainThreadDispatcher instance;

        public int MainThreadId { get; private set; }

        public static MainThreadDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("Main Thread Dispatcher").AddComponent<MainThreadDispatcher>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            MainThreadId = Thread.CurrentThread.ManagedThreadId;

            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            lock (ExecutionQueue)
            {
                while (ExecutionQueue.Count > 0) { ExecutionQueue.Dequeue().Invoke(); }
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void Dispatch(IEnumerator action)
        {
            lock (ExecutionQueue) { ExecutionQueue.Enqueue(() => { StartCoroutine(action); }); }
        }

        public void Dispatch(Action action)
        {
            Dispatch(ActionWrapper(action));
        }

        private static IEnumerator ActionWrapper(Action a)
        {
            a?.Invoke();
            yield return null;
        }
    }
}