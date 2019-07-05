using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Hermit.UIStack
{
    public class DefaultWidgetFactory : IWidgetFactory
    {
        public async Task<Widget> CreateInstance(IUIStack manager, string name, UIMessage message)
        {
            var loader = Her.Resolve<IViewLoader>();
            var prefab = await loader.LoadView(name);
            var instance = Object.Instantiate(prefab).GetComponent<Widget>();
            instance.SetManagerInfo(name, manager, message);
            return instance;
        }

        public void ReturnInstance(Widget widget)
        {
            widget.CleanUpViewInfo();
            Object.Destroy(widget.gameObject);
        }
    }
}