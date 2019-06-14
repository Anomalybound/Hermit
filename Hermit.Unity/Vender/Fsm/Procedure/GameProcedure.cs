using System;
using System.Globalization;

namespace Hermit.Procedure
{
    public abstract class
        GameProcedure<TProcedureController, TProcedureIndex> : ProcedureBase<TProcedureController>
        where TProcedureController : GameProcedureController<TProcedureController, TProcedureIndex>
        where TProcedureIndex : struct, IConvertible
    {
        public abstract TProcedureIndex Index { get; }

        public override void SetContext(TProcedureController context)
        {
            base.SetContext(context);
            Init(context);
        }

        public sealed override void Enter()
        {
            base.Enter();
            Enter(Context);
        }

        public sealed override void Exit()
        {
            base.Exit();
            Exit(Context);
        }

        public sealed override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Update(Context, deltaTime);
        }

        protected virtual void Init(TProcedureController controller) { }

        protected virtual void Enter(TProcedureController controller) { }

        protected virtual void Exit(TProcedureController controller) { }

        protected virtual void Update(TProcedureController controller, float deltaTime) { }

        #region Facade

        public void ChangeState(TProcedureIndex index)
        {
            Context.ChangeState(index.ToString(CultureInfo.InvariantCulture));
        }

        public void PushState(TProcedureIndex index)
        {
            Context.PushState(index.ToString(CultureInfo.InvariantCulture));
        }

        public new void ChangeState(string stateName)
        {
            Context.ChangeState(stateName);
        }

        public new void PushState(string stateName)
        {
            Context.PushState(stateName);
        }

        public new void PopState()
        {
            Context.PopState();
        }

        public new void TriggerEvent(string eventId, EventArgs args)
        {
            Context.TriggerEvent(eventId, args);
        }

        #endregion
    }
}