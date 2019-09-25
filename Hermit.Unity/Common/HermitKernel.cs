using Hermit.Injection;

namespace Hermit.Common
{
    [ScriptOrder(-10000)]
    public class HermitKernel : SceneContext
    {
        private static HermitKernel _instance;

        protected override void Awake()
        {
            if (_instance == null) { _instance = this; }
            else
            {
                Destroy(gameObject);
                return;
            }

            RegisterServices();
            InitServices();

            Inject(Her.Current);
        }
    }
}