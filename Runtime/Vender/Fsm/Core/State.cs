using System;
using System.Collections.Generic;

namespace Hermit.Fsm
{
    public class State : IBuildableState
    {
        #region IState Implementation

        public IState Parent { get; set; }

        public float ElapsedTime { get; private set; }

        public Dictionary<string, IState> Children { get; } = new Dictionary<string, IState>();

        public Stack<IState> ActiveChildrenStates { get; } = new Stack<IState>();

        public virtual void Enter()
        {
            OnEnterAction?.Invoke();

            ElapsedTime = 0f;
        }

        public virtual void Update(float deltaTime)
        {
            // Only update the latest state
            if (ActiveChildrenStates.Count > 0)
            {
                ActiveChildrenStates.Peek().Update(deltaTime);
                return;
            }

            OnUpdateAction?.Invoke(deltaTime);

            ElapsedTime += deltaTime;

            // Check if condition meets
            foreach (var conditionPair in _conditions)
            {
                if (conditionPair.Key.Invoke()) { conditionPair.Value?.Invoke(); }
            }
        }

        public virtual void Exit()
        {
            OnExitAction?.Invoke();
        }

        public virtual void ChangeState(string name)
        {
            if (!Children.TryGetValue(name, out var result))
            {
                throw new ApplicationException($"Child state [{name}] not found.");
            }

            while (ActiveChildrenStates.Count > 0) { PopState(); }

            InternalPushState(result);
        }

        public void PushState(string name)
        {
            if (!Children.TryGetValue(name, out var result))
            {
                throw new ApplicationException($"Child state [{name}] not found.");
            }

            InternalPushState(result);
        }

        public void PopState()
        {
            InternalPopState();
        }

        public void TriggerEvent(string id)
        {
            TriggerEvent(id, EventArgs.Empty);
        }

        public void TriggerEvent(string id, EventArgs eventArgs)
        {
            if (ActiveChildrenStates.Count > 0)
            {
                ActiveChildrenStates.Peek().TriggerEvent(id, eventArgs);
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

        public event Action OnEnterAction;

        public event Action OnExitAction;

        public event Action<float> OnUpdateAction;

        #endregion

        #region Runtime

        private readonly Dictionary<string, Action<EventArgs>> _events =
            new Dictionary<string, Action<EventArgs>>();

        private readonly Dictionary<Func<bool>, Action> _conditions = new Dictionary<Func<bool>, Action>();

        #endregion

        #region Private Operations

        private void InternalPopState()
        {
            if (ActiveChildrenStates.Count <= 0)
            {
                throw new IndexOutOfRangeException($"ActiveStates stack is empty.");
            }

            var result = ActiveChildrenStates.Pop();
            result.Exit();
        }

        private void InternalPushState(IState state)
        {
            ActiveChildrenStates.Push(state);
            state.Enter();
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

        public void SetEnterAction(Action onEnterAction)
        {
            OnEnterAction = onEnterAction;
        }

        public void SetExitAction(Action onExitAction)
        {
            OnExitAction = onExitAction;
        }

        public void SetUpdateAction(Action<float> onUpdateAction)
        {
            OnUpdateAction = onUpdateAction;
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
