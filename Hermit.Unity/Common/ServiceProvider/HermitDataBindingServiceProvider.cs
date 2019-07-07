using Hermit.DataBinding;
using Hermit.Injection;

namespace Hermit.Common
{
    public class HermitDataBindingServiceProvider : MonoServiceProvider
    {
        public override void RegisterBindings(IDependencyContainer Container)
        {
            var adapterTypes = AssemblyHelper.GetInheritancesInParentAssembly(typeof(IAdapter));
            foreach (var adapterType in adapterTypes)
            {
                Container.Bind<IAdapter>().To(adapterType).WithId(adapterType.FullName);
            }

            var viewHandlers = AssemblyHelper.GetInheritancesInParentAssembly(typeof(IViewCollectionChangedHandler));
            foreach (var viewHandler in viewHandlers)
            {
                Container.Bind<IViewCollectionChangedHandler>().To(viewHandler)
                    .AsTransient().WithId(viewHandler.FullName);
            }
        }
    }
}