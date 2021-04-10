using System;

namespace Hermit.Fsm
{
    public abstract class
        GameProcedure<TProcedureController, TProcedureIndex> : ProcedureBase<TProcedureController>
        where TProcedureController : GameProcedureController<TProcedureController, TProcedureIndex>
        where TProcedureIndex : struct, IConvertible
    {
        public abstract TProcedureIndex Index { get; }

        public override void SetContext(TProcedureController controller)
        {
            base.SetContext(controller);
            OnInit();
        }

        public sealed override void Enter()
        {
            base.Enter();
            OnEnter();
        }

        public sealed override void Exit()
        {
            base.Exit();
            OnExit();
        }

        public sealed override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            OnUpdate(deltaTime);
        }

        protected virtual void OnInit() { }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }

        protected virtual void OnUpdate(float deltaTime) { }
    }
}
