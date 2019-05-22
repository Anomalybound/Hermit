using System;
using Hermit.Injection;
using Hermit.UIStack;

namespace Hermit
{
    public sealed partial class Her
    {
        public static Version Version = new Version("0.0.1");

        private static Her Current
        {
            get
            {
                if (current == null) { throw new Exception("Please add KarmaKernel to your bootstrap scene."); }

                return current;
            }
        }

        private static Her current;

        private readonly IDependencyContainer _container;

        private readonly IEventBroker _eventBroker;

        private readonly IUIStack _iuiStack;

        private readonly ILog _logger;

        public Her(IDependencyContainer container, IEventBroker eventBroker, IUIStack iuiStack, ILog logger)
        {
            _container = container;
            _eventBroker = eventBroker;
            _iuiStack = iuiStack;
            _logger = logger;

            current = this;
        }
    }
}