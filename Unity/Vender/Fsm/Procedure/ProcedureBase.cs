using Hermit.Fsm;

namespace Hermit.Procedure
{
    public abstract class ProcedureBase<TProcedureController> : ContextState<TProcedureController>
    {
        public override TProcedureController Controller { get; protected set; }

        public override void SetContext(TProcedureController controller)
        {
            Controller = controller;
        }
    }
}