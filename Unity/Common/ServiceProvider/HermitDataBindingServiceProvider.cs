using Hermit.DataBinding;
using Hermit.Injection;

namespace Hermit.Common
{
    public class HermitDataBindingServiceProvider : ServiceProviderBase
    {
        public override void RegisterBindings(IDependencyContainer container)
        {
            var adapterTypes = AssemblyHelper.GetInheritancesInAppDomain(typeof(IAdapter));
            foreach (var adapterType in adapterTypes)
            {
                container.Bind<IAdapter>().To(adapterType).WithId(adapterType.FullName);
            }

            var viewHandlers = AssemblyHelper.GetInheritancesInAppDomain(typeof(IViewCollectionChangedHandler));
            foreach (var viewHandler in viewHandlers)
            {
                container.Bind<IViewCollectionChangedHandler>().To(viewHandler)
                    .AsTransient().WithId(viewHandler.FullName);
            }
        }
    }
}