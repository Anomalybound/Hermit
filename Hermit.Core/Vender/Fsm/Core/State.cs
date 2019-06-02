using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hermit.Fsm
{
    public class State : IBuildableState
    {
        #region IState Implementation

        public IState Parent { get; set; }

        public float ElapsedTime { get; private set; }

        public Dictionary<string, IState> Children { get; } = new Dictionary<string, IState>();

        public Stack<IState> ActiveStates { get; } = new Stack<IState>();

        public virtual async Task Enter()
        {
            await Task.Run(() =>
            {
                OnEnter?.Invoke();
                
                ElapsedTime = 0f;
            });
        }

        public virtual void Update(float deltaTime)
        {
            // Only update the latest state
            if (ActiveStates.Count > 0)
            {
                ActiveStates.Peek().Update(deltaTime);
                return;
            }

            OnUpdate?.Invoke(deltaTime);

            ElapsedTime += deltaTime;

            // Check if condition meets
            foreach (var conditionPair in _conditions)
            {
                if (conditionPair.Key.Invoke()) { conditionPair.Value?.Invoke(); }
            }
        }

        public virtual async Task Exit()
        {
            await Task.Run(() => OnExit?.Invoke());
        }

        public virtual async Task ChangeState(string name)
        {
            if (!Children.TryGetValue(name, out var result))
            {
                throw new ApplicationException($"Child state [{name}] not found.");
            }

            if (ActiveStates.Count > 0) { await PopState(); }

            await InternalPushState(result);
        }

        public async Task PushState(string name)
        {
            if (!Children.TryGetValue(name, out var result))
            {
                throw new ApplicationException($"Child state [{name}] not found.");
            }

            await InternalPushState(result);
        }

        public async Task PopState()
        {
            await PrivatePopState();
        }

        public async Task TriggerEvent(string id)
        {
            await TriggerEvent(id, EventArgs.Empty);
        }

        public async Task TriggerEvent(string id, EventArgs eventArgs)
        {
            if (ActiveStates.Count > 0)
            {
                await ActiveStates.Peek().TriggerEvent(id, eventArgs);
                return;
            }

            if (!_events.TryGetValue(id, out var action))
            {
                throw new ApplicationException($"Event [{id}] not exits.");
            }

            action?.Invoke(eventArgs);
        }

        #endregion

        #region Actions

        public event Action OnEnter;
        public event Action OnExit;
        public event Action<float> OnUpdate;

        #endregion

        #region Runtime

        private readonly Dictionary<string, Action<EventArgs>> _events = new Dictionary<string, Action<EventArgs>>();
        private readonly Dictionary<Func<bool>, Action> _conditions = new Dictionary<Func<bool>, Action>();

        #endregion

        #region Private Operations

        private async Task PrivatePopState()
        {
            var result = ActiveStates.Pop();
            await result.Exit();
        }

        private async Task InternalPushState(IState state)
        {
            ActiveStates.Push(state);
            await state.Enter();
        }

        #endregion

        #region Helper

        public void AddChild(string name, IState state)
        {
            if (!Children.ContainsKey(name))
            {
                Children.Add(name, state);
                state.Parent = this;
            }
            else { throw new ApplicationException($"Child state already exists: {name}"); }
        }

        public void SetEnterAction(Action onEnter)
        {
            OnEnter = onEnter;
        }

        public void SetExitAction(Action onExit)
        {
            OnExit = onExit;
        }

        public void SetUpdateAction(Action<float> onUpdate)
        {
            OnUpdate = onUpdate;
        }

        public void AddEvent(string id, Action<EventArgs> action)
        {
            if (!_events.ContainsKey(id)) { _events.Add(id, action); }
            else { throw new ApplicationException($"Event already exists: {id}"); }
        }

        public void AddEvent<TArgs>(string id, Action<TArgs> action) where TArgs : EventArgs
        {
            if (!_events.ContainsKey(id)) { _events.Add(id, arg => { action.Invoke((TArgs) arg); }); }
            else { throw new ApplicationException($"Event already exists: {id}"); }
        }

        public void AddCondition(Func<bool> predicate, Action action)
        {
            _conditions.Add(predicate, action);
        }

        #endregion
    }
}