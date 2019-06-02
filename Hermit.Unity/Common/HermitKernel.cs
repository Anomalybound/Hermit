using Hermit.Injection;

namespace Hermit.Common
{
    [ScriptOrder(-10000)]
    public class HermitKernel : SceneContext
    {
        public static Her HerInstance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            HerInstance = Singleton<Her>();
        }
    }
}