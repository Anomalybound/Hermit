using Hermit.DataBinding;
using Hermit.Injection;

namespace Hermit.Common
{
    public class HermitBindingAdapterModule : MonoModule
    {
        public override void RegisterBindings(IDependencyContainer Container)
        {
            var adapterTypes = AssemblyHelper.GetInheritancesInParentAssembly(typeof(IAdapter));
            foreach (var adapterType in adapterTypes)
            {
                Container.Bind<IAdapter>().To(adapterType).WithId(adapterType.FullName);
            }
        }
    }
}