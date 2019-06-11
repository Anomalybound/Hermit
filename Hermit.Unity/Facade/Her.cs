using System;
using Hermit.Injection;
using Hermit.UIStack;

namespace Hermit
{
    public sealed partial class Her
    {
        public static Version Version = new Version("0.1.4");

        private static Her Current
        {
            get
            {
                if (current == null) { throw new Exception("Please add KarmaKernel to your bootstrap scene."); }

                return current;
            }
        }

        private static Her current;

        private IDependencyContainer container { get; }

        private IEventBroker eventBroker { get; }

        private IUIStack uiStack { get; }

        private ILog logger { get; }

        public Her(IDependencyContainer container, IEventBroker eventBroker, IUIStack uiStack, ILog logger)
        {
            this.container = container;
            this.eventBroker = eventBroker;
            this.uiStack = uiStack;
            this.logger = logger;

            var store = container.Resolve<IStore>();
            store.SetStoreId("Global");
            stores.Add("Global", store);

            current = this;
        }
    }
}