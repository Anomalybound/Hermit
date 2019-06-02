using System;

namespace Hermit.Fsm
{
    public class StateBuilder<TState, TParentBuilder> where TState : IBuildableState, new()
    {
        public TState ContractState { get; }
        
        public TParentBuilder ParentBuilder { get; }

        public StateBuilder(string name, TParentBuilder parentBuilder, IBuildableState parent)
        {
            ParentBuilder = parentBuilder;
            ContractState = new TState();
            parent.AddChild(name, ContractState);
        }

        #region Fluent Actions

        public StateBuilder<State, StateBuilder<TState, TParentBuilder>> State(string name)
        {
            return new StateBuilder<State, StateBuilder<TState, TParentBuilder>>(name, this, ContractState);
        }

        public StateBuilder<T, StateBuilder<TState, TParentBuilder>> State<T>(string name) where T : IBuildableState, new()
        {
            return new StateBuilder<T, StateBuilder<TState, TParentBuilder>>(name, this, ContractState);
        }

        public StateBuilder<TState, TParentBuilder> OnEnter(Action<TState> action)
        {
            ContractState.SetEnterAction(() => action(ContractState));
            return this;
        }

        public StateBuilder<TState, TParentBuilder> OnExit(Action<TState> action)
        {
            ContractState.SetExitAction(() => action(ContractState));
            return this;
        }

        public StateBuilder<TState, TParentBuilder> OnUpdate(Action<TState, float> action)
        {
            ContractState.SetUpdateAction(deltaTime => action(ContractState, deltaTime));
            return this;
        }

        public StateBuilder<TState, TParentBuilder> Condition(Func<bool> predicate, Action<TState> action)
        {
            ContractState.AddCondition(predicate, () => action(ContractState));
            return this;
        }

        public StateBuilder<TState, TParentBuilder> Event(string id, Action<TState, EventArgs> action)
        {
            ContractState.AddEvent(id, args => action(ContractState, args));
            return this;
        }

        public StateBuilder<TState, TParentBuilder> Event<TArgs>(string id, Action<TState, TArgs> action)
            where TArgs : EventArgs
        {
            ContractState.AddEvent(id, args => action(ContractState, (TArgs) args));
            return this;
        }

        public TParentBuilder End()
        {
            return ParentBuilder;
        }

        #region Context State

        public StateBuilder<TState, StateBuilder<TState, TParentBuilder>> State<TContext, TContextState>(string stateName, TContext context)
            where TContextState : ContextState<TContext>, TState, new()
        {
            var builder = new StateBuilder<TContextState, StateBuilder<TState, TParentBuilder>>(stateName, this, ContractState);
            builder.ContractState.SetContext(context);
            return builder as StateBuilder<TState, StateBuilder<TState, TParentBuilder>>;
        }

        #endregion

        #endregion
    }
}