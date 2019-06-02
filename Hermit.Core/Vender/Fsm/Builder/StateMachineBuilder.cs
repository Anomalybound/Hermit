namespace Hermit.Fsm
{
    public class StateMachineBuilder<TState> where TState : IBuildableState, new()
    {
        private readonly IBuildableState _root;

        public StateMachineBuilder()
        {
            _root = new TState();
        }

        #region Fluent Actions

        public StateBuilder<TState, StateMachineBuilder<TState>> State(string stateName)
        {
            return new StateBuilder<TState, StateMachineBuilder<TState>>(stateName, this, _root);
        }

        public StateBuilder<T, StateMachineBuilder<TState>> State<T>(string stateName) where T : IBuildableState, new()
        {
            return new StateBuilder<T, StateMachineBuilder<TState>>(stateName, this, _root);
        }

        public IBuildableState Build()
        {
            return _root;
        }

        #region Context State

        public StateBuilder<TState, StateMachineBuilder<TState>> State<TContext, TContextState>(string stateName,
            TContext context)
            where TContextState : ContextState<TContext>, TState, new()
        {
            var builder = new StateBuilder<TContextState, StateMachineBuilder<TState>>(stateName, this, _root);
            builder.ContractState.SetContext(context);
            return builder as StateBuilder<TState, StateMachineBuilder<TState>>;
        }

        #endregion

        #endregion
    }
}