using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Hermit.Procedure
{
    public abstract class
        GameProcedure<TProcedureController, TProcedureIndex> : ProcedureBase<TProcedureController>
        where TProcedureController : GameProcedureController<TProcedureController, TProcedureIndex>
        where TProcedureIndex : struct, IConvertible
    {
        public abstract TProcedureIndex Index { get; }

        public override async Task SetContext(TProcedureController context)
        {
            await base.SetContext(context);
            await Init(context);
        }

        public sealed override async Task Enter()
        {
            await base.Enter();
            await Enter(Context);
        }

        public sealed override async Task Exit()
        {
            await base.Exit();
            await Exit(Context);
        }

        public sealed override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Update(Context, deltaTime);
        }

        public virtual async Task Init(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        public virtual async Task Enter(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        public virtual async Task Exit(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        public virtual void Update(TProcedureController controller, float deltaTime) { }

        #region Facade

        public async Task ChangeState(TProcedureIndex index)
        {
            await Context.ChangeState(index.ToString(CultureInfo.InvariantCulture));
        }

        public async Task PushState(TProcedureIndex index)
        {
            await Context.PushState(index.ToString(CultureInfo.InvariantCulture));
        }

        public new async Task ChangeState(string stateName)
        {
            await Context.ChangeState(stateName);
        }

        public new async Task PushState(string stateName)
        {
            await Context.PushState(stateName);
        }

        public new async Task PopState()
        {
            await Context.PopState();
        }

        public new async Task TriggerEvent(string eventId, EventArgs args)
        {
            await Context.TriggerEvent(eventId, args);
        }

        #endregion
    }
}