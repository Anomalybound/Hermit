using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Hermit.UIStack
{
    [CustomWidgetFactory(typeof(Widget))]
    public class DefaultWidgetFactory : IWidgetFactory<Widget>
    {
        public async Task<Widget> CreateInstance(IUIStack manager, string name, int assignedId,
            UIMessage message)
        {
            var loader = Her.Resolve<IViewLoader>();
            var prefab = await loader.LoadView(name);
            var instance = Object.Instantiate(prefab).GetComponent<Widget>();
            instance.SetManagerInfo(assignedId, name, manager, message);
            Her.Inject(instance);
            instance.OnDestroyEvent += ReturnInstance;
            return instance;
        }

        public void ReturnInstance(Widget widget)
        {
            widget.OnDestroyEvent -= ReturnInstance;
            Object.Destroy(widget);
        }

        async Task<IWidget> IWidgetFactory.CreateInstance(IUIStack manager, string name, int assignedId,
            UIMessage message)
        {
            var loader = Her.Resolve<IViewLoader>();
            var prefab = await loader.LoadView(name);
            var instance = Object.Instantiate(prefab).GetComponent<IWidget>();
            instance.SetManagerInfo(assignedId, name, manager, message);
            Her.Inject(instance);
            return instance;
        }
    }
}