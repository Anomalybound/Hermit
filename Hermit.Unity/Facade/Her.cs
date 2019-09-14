using System;
using Hermit.Injection;
using Hermit.UIStack;

namespace Hermit
{
    public sealed partial class Her
    {
        public static readonly Version Version = new Version("0.2.2");

        private static Her Current
        {
            get
            {
                if (current == null) { throw new Exception("Please add KarmaKernel to your bootstrap scene."); }

                return current;
            }
        }

        private static Her current;

        private IDependencyContainer _container { get; }

        private IEventBroker _eventBroker { get; }

        private IUIStack _uiStack { get; }

        private ILog _logger { get; }

        private IViewManager _viewManager { get; }

        public Her(IDependencyContainer container, IEventBroker eventBroker, IUIStack uiStack, ILog logger,
            IViewManager viewManager)
        {
            _container = container;
            _eventBroker = eventBroker;
            _uiStack = uiStack;
            _logger = logger;
            _viewManager = viewManager;

            // Setup stores
            var store = container.Resolve<IStore>();
            store.SetStoreId("Global");
            stores.Add("Global", store);

            current = this;
        }
    }
}