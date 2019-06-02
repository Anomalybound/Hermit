using Hermit.Fsm;
using UnityEngine;
using System.Threading.Tasks;

namespace Hermit
{
    public abstract class FsmContainer : MonoBehaviour
    {
        public IState Root { get; protected set; }

        private bool _running;

        public abstract Task<IState> BuildState();

        protected virtual async void Awake()
        {
            Root = await BuildState();
        }

        protected virtual void OnEnable()
        {
            _running = true;
        }

        protected virtual void OnDisable()
        {
            _running = false;
        }

        protected virtual void Update()
        {
            if (_running) { Root?.Update(Time.deltaTime); }
        }
    }
}