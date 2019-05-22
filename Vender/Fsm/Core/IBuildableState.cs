using System;

namespace Hermit.Fsm
{
    public interface IBuildableState : IState
    {
        void AddChild(string name, IState state);

        void SetEnterAction(Action onEnter);

        void SetExitAction(Action onExit);

        void SetUpdateAction(Action<float> onUpdate);

        void AddEvent(string id, Action<EventArgs> action);

        void AddEvent<TArgs>(string id, Action<TArgs> action) where TArgs : EventArgs;

        void AddCondition(Func<bool> predicate, Action action);
    }
}