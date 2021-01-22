namespace Hermit.Fsm
{
    public abstract class ContextState<TContext> : State
    {
        public abstract TContext Controller { get; protected set; }

        public abstract void SetContext(TContext controller);
    }
}