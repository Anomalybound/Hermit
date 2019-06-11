using Hermit.Fsm;
using UnityEngine;
using System.Threading.Tasks;

namespace Hermit
{
    public abstract class FsmContainer : MonoBehaviour
    {
        public bool UseFixedUpdate;

        public IState Root { get; protected set; }

        protected bool Running { get; private set; }

        protected abstract Task<IState> BuildState();

        protected virtual void OnInit() { }

        protected virtual void OnUpdate(float deltaTime) { }

        protected virtual void OnFixedUpdate(float deltaTime) { }

        protected virtual void OnResume() { }

        protected virtual void OnPause() { }

        private async void Awake()
        {
            OnInit();
            Root = await BuildState();
        }

        private void OnEnable()
        {
            Running = true;
            OnResume();
        }

        private void OnDisable()
        {
            Running = false;
            OnPause();
        }

        private void Update()
        {
            if (Running && !UseFixedUpdate) { Root?.Update(Time.deltaTime); }

            OnUpdate(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (Running && UseFixedUpdate) { Root?.Update(Time.fixedDeltaTime); }

            OnFixedUpdate(Time.fixedDeltaTime);
        }
    }
}