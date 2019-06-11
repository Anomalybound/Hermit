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

        public sealed override async Task EnterAsync()
        {
            await base.EnterAsync();
            await Enter(Context);
        }

        public sealed override async Task ExitAsync()
        {
            await base.ExitAsync();
            await Exit(Context);
        }

        public sealed override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Update(Context, deltaTime);
        }

        protected virtual async Task Init(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        protected virtual async Task Enter(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        protected virtual async Task Exit(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        protected virtual void Update(TProcedureController controller, float deltaTime) { }

        #region Facade

        public async Task ChangeState(TProcedureIndex index)
        {
            await Context.ChangeState(index.ToString(CultureInfo.InvariantCulture));
        }

        public async Task PushState(TProcedureIndex index)
        {
            await Context.PushState(index.ToString(CultureInfo.InvariantCulture));
        }

        public new async Task ChangeStateAsync(string stateName)
        {
            await Context.ChangeState(stateName);
        }

        public new async Task PushStateAsync(string stateName)
        {
            await Context.PushState(stateName);
        }

        public new async Task PopStateAsync()
        {
            await Context.PopState();
        }

        public new async Task TriggerEventAsync(string eventId, EventArgs args)
        {
            await Context.TriggerEvent(eventId, args);
        }

        #endregion
    }
}