using System.Threading.Tasks;
using Hermit.Fsm;

namespace Hermit.Procedure
{
    public abstract class ProcedureBase<TProcedureManager> : ContextState<TProcedureManager>
    {
        public override TProcedureManager Context { get; protected set; }

        public override async Task SetContext(TProcedureManager context)
        {
            await Task.Run(() => Context = context);
        }
    }
}