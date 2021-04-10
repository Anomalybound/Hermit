using System;
using System.Collections.Generic;

namespace Hermit.Fsm
{
    public class State : IBuildableState
    {
        #region IState Implementation

        public bool Active { get; private set; }

        public IState Parent { get; set; }

        public float ElapsedTime { get; private set; }
        public Dictionary<string, IState> Children { get; } = new Dictionary<string, IState>();

        public Stack<IState> ActiveChildrenStates { get; } = new Stack<IState>();

        #region Lifecycle

        public virtual void Enter()
        {
            OnEnterAction?.Invoke();

            ElapsedTime = 0f;

            Active = true;
        }

        public virtual void Update(float deltaTime)
        {
            // Only update the latest state
            if (ActiveChildrenStates.Count > 0)
            {
                ActiveChildrenStates.Peek().Update(deltaTime);
            }

            OnUpdateAction?.Invoke(deltaTime);

            ElapsedTime += deltaTime;

            // Check if any condition meets
            foreach (var conditionPair in _conditions)
            {
                if (conditionPair.Key.Invoke())
                {
                    conditionPair.Value?.Invoke();
                }
            }
        }

        public virtual void Exit()
        {
            InternalPopAll(false);

            OnExitAction?.Invoke();

            Active = false;
        }

        #endregion

        public void ClearStates()
        {
            InternalPopAll(true);
            Enter();
        }

        public virtual void ChangeState(string name)
        {
            if (!Children.TryGetValue(name, out var result))
            {
                throw new ApplicationException($"Child state [{name}] not found.");
            }

            while (ActiveChildrenStates.Count > 0)
            {
                PopState(true);
            }

            InternalPushState(result, false);
        }

        public void PushState(string name, bool allowDuplicated = false)
        {
            if (!Children.TryGetValue(name, out var result))
            {
                throw new ApplicationException($"Child state [{name}] not found.");
            }

            InternalPushState(result, allowDuplicated);
        }

        public void PopState(bool backToRoot)
        {
            InternalPopState();

            var active = ActiveChildrenStates.Count > 0 ? ActiveChildrenStates.Peek() :
                backToRoot ? this : null;
            active?.Enter();
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

        #region Internal Operations

        private void InternalPopAll(bool enterRoot)
        {
            while (ActiveChildrenStates.Count > 0)
            {
                PopState(enterRoot);
            }
        }

        private void InternalPopState()
        {
            if (ActiveChildrenStates.Count <= 0)
            {
                throw new IndexOutOfRangeException($"ActiveStates stack is empty.");
            }

            var result = ActiveChildrenStates.Pop();
            result.Exit();
        }

        private void InternalPushState(IState state, bool allowDuplicated)
        {
            if (ActiveChildrenStates.Count > 0 && !allowDuplicated)
            {
                var activeChild = ActiveChildrenStates.Peek();
                if (activeChild == state)
                {
                    return;
                }
            }

            var active = ActiveChildrenStates.Count > 0 ? ActiveChildrenStates.Peek() : null;
            active?.Exit();

            ActiveChildrenStates.Push(state);
            state.Enter();
        }

        #endregion

        #region Helper

        public TState AddChild<TState>(string name, TState state) where TState : IState
        {
            if (!Children.ContainsKey(name))
            {
                Children.Add(name, state);
                state.Parent = this;
            }
            else
            {
                throw new ApplicationException($"Child state already exists: {name}");
            }

            return state;
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
            if (!_events.ContainsKey(id))
            {
                _events.Add(id, action);
            }
            else
            {
                throw new ApplicationException($"Event already exists: {id}");
            }
        }

        public void AddEvent<TArgs>(string id, Action<TArgs> action) where TArgs : EventArgs
        {
            if (!_events.ContainsKey(id))
            {
                _events.Add(id, arg => { action.Invoke((TArgs) arg); });
            }
            else
            {
                throw new ApplicationException($"Event already exists: {id}");
            }
        }

        public void AddCondition(Func<bool> predicate, Action action)
        {
            _conditions.Add(predicate, action);
        }

        #endregion
    }
}
