using Hermit.Injection;

namespace Hermit.Common
{
    [ScriptOrder(-10000)]
    public sealed class HermitKernel : SceneContext
    {
        private static HermitKernel instance;

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;
                base.Awake();
                Inject(Her.Current);
            }
            else
            {
                Her.Warn("HermitKernel can only have one instance.");
                Destroy(gameObject);
            }
        }
    }
}