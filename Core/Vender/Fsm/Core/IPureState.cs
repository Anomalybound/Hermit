namespace Hermit.Fsm.Core
{
    public interface IPureState
    {
        #region Lifetime

        void Enter();

        void Update(float deltaTime);

        void Exit();

        float ElapsedTime { get; }

        #endregion
    }
}