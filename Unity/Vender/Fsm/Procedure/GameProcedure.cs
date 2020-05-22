using System;
using System.Globalization;

namespace Hermit.Fsm.Procedure
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

        #region Facade

        public void ChangeState(TProcedureIndex index)
        {
            Controller.ChangeState(index.ToString(CultureInfo.InvariantCulture));
        }

        public void PushState(TProcedureIndex index)
        {
            Controller.PushState(index.ToString(CultureInfo.InvariantCulture));
        }

        public new void ChangeState(string stateName)
        {
            Controller.ChangeState(stateName);
        }

        public new void PushState(string stateName)
        {
            Controller.PushState(stateName);
        }

        public new void PopState()
        {
            Controller.PopState();
        }

        public new void TriggerEvent(string eventId, EventArgs args)
        {
            Controller.TriggerEvent(eventId, args);
        }

        #endregion
    }
}