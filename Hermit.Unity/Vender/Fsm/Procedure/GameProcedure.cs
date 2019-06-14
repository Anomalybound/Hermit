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
            await InitAsync(context);
        }

        public sealed override async Task EnterAsync()
        {
            await base.EnterAsync();
            await EnterAsync(Context);
        }

        public sealed override async Task ExitAsync()
        {
            await base.ExitAsync();
            await ExitAsync(Context);
        }

        public sealed override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Update(Context, deltaTime);
        }

        protected virtual async Task InitAsync(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        protected virtual async Task EnterAsync(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        protected virtual async Task ExitAsync(TProcedureController controller)
        {
            await Task.FromResult(default(object));
        }

        protected virtual void Update(TProcedureController controller, float deltaTime) { }

        #region Facade

        public async Task ChangeStateAsync(TProcedureIndex index)
        {
            await Context.ChangeStateAsync(index.ToString(CultureInfo.InvariantCulture));
        }

        public async Task PushStateAsync(TProcedureIndex index)
        {
            await Context.PushStateAsync(index.ToString(CultureInfo.InvariantCulture));
        }

        public new async Task ChangeStateAsync(string stateName)
        {
            await Context.ChangeStateAsync(stateName);
        }

        public new async Task PushStateAsync(string stateName)
        {
            await Context.PushStateAsync(stateName);
        }

        public new async Task PopStateAsync()
        {
            await Context.PopStateAsync();
        }

        public new async Task TriggerEventAsync(string eventId, EventArgs args)
        {
            await Context.TriggerEventAsync(eventId, args);
        }

        #endregion
    }
}