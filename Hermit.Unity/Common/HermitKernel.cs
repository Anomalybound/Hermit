using Hermit.Injection;

namespace Hermit.Common
{
    [ScriptOrder(-10000)]
    public sealed class HermitKernel : SceneContext
    {
        private static HermitKernel _instance;

        protected override void Awake()
        {
            if (_instance == null)
            {
                _instance = this;

                RegisterServices();
                InitServices();
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