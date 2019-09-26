using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IWidgetFactory
    {
         Task<Widget> CreateInstance(IUIStack manager, string name, UIMessage message);

        void ReturnInstance(Widget widget);
    }
}